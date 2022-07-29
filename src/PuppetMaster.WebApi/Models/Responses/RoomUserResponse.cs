namespace PuppetMaster.WebApi.Models.Responses
{
    public class RoomUserResponse
    {
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public Guid? ModifiedBy { get; set; }

        public Guid ApplicationUserId { get; set; }

        public UserResponse? ApplicationUser { get; set; }

        public Guid RoomId { get; set; }

        public bool IsReady { get; set; }
    }
}
