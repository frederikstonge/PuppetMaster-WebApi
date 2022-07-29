using System.ComponentModel.DataAnnotations;

namespace PuppetMaster.WebApi.Models.Database
{
    public abstract class EntityBase
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public Guid? ModifiedBy { get; set; }
    }
}
