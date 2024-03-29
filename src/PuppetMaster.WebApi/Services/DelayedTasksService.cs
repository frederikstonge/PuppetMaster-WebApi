﻿using System.Collections.Concurrent;

namespace PuppetMaster.WebApi.Services
{
    public class DelayedTasksService : IDelayedTasksService
    {
        private static readonly ConcurrentDictionary<Guid, CancellationTokenSource> TaskTokens = new ();
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DelayedTasksService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void CancelTask(Guid roomId)
        {
            if (TaskTokens.Remove(roomId, out var tokenSource))
            {
                tokenSource!.Cancel();
            }
        }

        public void SchedulePlayerPick(Guid userId, Guid matchId, Guid roomId, TimeSpan delay)
        {
            if (TaskTokens.ContainsKey(roomId))
            {
                return;
            }

            var tokenSource = new CancellationTokenSource();
            TaskTokens.TryAdd(roomId, tokenSource);

            Task.Run(
                async () =>
                {
                    try
                    {
                        await Task.Delay(delay, tokenSource.Token);
                        if (tokenSource.IsCancellationRequested)
                        {
                            return;
                        }

                        using var scope = _serviceScopeFactory.CreateScope();
                        var matchesService = scope.ServiceProvider.GetRequiredService<IMatchesService>();
                        var match = await matchesService.GetMatchAsync(matchId);
                        var pickedPlayers = match.MatchTeams!.SelectMany(mt => mt.MatchTeamUsers!.Select(mtu => mtu.ApplicationUserId));
                        var availablePlayers = match.Users!.ExceptBy(pickedPlayers, u => u!.Id).ToList();
                        var random = new Random();
                        var pickIndex = random.Next(availablePlayers.Count - 1);
                        var pickPlayer = availablePlayers.ElementAt(pickIndex);
                        if (tokenSource.IsCancellationRequested)
                        {
                            return;
                        }

                        await matchesService.PickPlayerAsync(userId, matchId, pickPlayer.Id);
                        TaskTokens.Remove(roomId, out var _);
                    }
                    finally
                    {
                        tokenSource.Dispose();
                    }
                }, tokenSource.Token);
        }

        public void ScheduleCreateLobby(Guid matchId, Guid roomId, TimeSpan delay)
        {
            if (TaskTokens.ContainsKey(roomId))
            {
                return;
            }

            var tokenSource = new CancellationTokenSource();
            TaskTokens.TryAdd(roomId, tokenSource);

            Task.Run(
                async () =>
                {
                    try
                    {
                        await Task.Delay(delay, tokenSource.Token);
                        if (tokenSource.IsCancellationRequested)
                        {
                            return;
                        }

                        using var scope = _serviceScopeFactory.CreateScope();
                        var matchesService = scope.ServiceProvider.GetRequiredService<IMatchesService>();
                        var match = await matchesService.GetMatchAsync(matchId);
                        var hubService = scope.ServiceProvider.GetRequiredService<IHubService>();
                        if (tokenSource.IsCancellationRequested)
                        {
                            return;
                        }

                        await hubService.OnCreateLobbyAsync(match);
                        TaskTokens.Remove(roomId, out var _);
                    }
                    finally
                    {
                        tokenSource.Dispose();
                    }
                }, tokenSource.Token);
        }

        public void HasJoinedTimeout(Guid matchId, Guid roomId, TimeSpan delay)
        {
            if (TaskTokens.ContainsKey(roomId))
            {
                return;
            }

            var tokenSource = new CancellationTokenSource();
            TaskTokens.TryAdd(roomId, tokenSource);

            Task.Run(
                async () =>
                {
                    try
                    {
                        await Task.Delay(delay, tokenSource.Token);
                        if (tokenSource.IsCancellationRequested)
                        {
                            return;
                        }

                        using var scope = _serviceScopeFactory.CreateScope();
                        var matchesService = scope.ServiceProvider.GetRequiredService<IMatchesService>();
                        var match = await matchesService.GetMatchAsync(matchId);
                        var hubService = scope.ServiceProvider.GetRequiredService<IHubService>();
                        if (tokenSource.IsCancellationRequested)
                        {
                            return;
                        }

                        var notJoined = match.MatchTeams!.SelectMany(mt => mt.MatchTeamUsers!).Where(mtu => !mtu.HasJoined);

                        if (notJoined.Any())
                        {
                            // Apply ban
                            await matchesService.MatchAbandonnedAsync(matchId);
                        }

                        TaskTokens.Remove(roomId, out var _);
                    }
                    finally
                    {
                        tokenSource.Dispose();
                    }
                }, tokenSource.Token);
        }
    }
}
