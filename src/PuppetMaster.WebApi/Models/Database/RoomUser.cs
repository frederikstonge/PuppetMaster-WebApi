namespace PuppetMaster.WebApi.Models.Database
{
    public class RoomUser : EntityBase
    {
        public Guid ApplicationUserId { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }

        public Guid RoomId { get; set; }

        public Room? Room { get; set; }

        public bool IsReady { get; set; }
    }
}