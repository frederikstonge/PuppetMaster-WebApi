using AutoMapper;
using PuppetMaster.WebApi.Models.Database;
using PuppetMaster.WebApi.Models.Responses;

namespace PuppetMaster.WebApi.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserResponse>()
                .ForMember(
                    u => u.AvatarUrl,
                    opt => opt.MapFrom(au => !string.IsNullOrEmpty(au.AvatarUrl) ? au.AvatarUrl : "/images/default-avatar.png"))
                .ForMember(
                    gu => gu.RoomUserId,
                    opt => opt.MapFrom(gur => (gur.RoomUser != null) ? gur.RoomUser.Id : (Guid?)null))
                .ForMember(
                    gu => gu.RoomId,
                    opt => opt.MapFrom(gur => (gur.RoomUser != null) ? gur.RoomUser.RoomId : (Guid?)null));
        }
    }
}
