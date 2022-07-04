using System.ComponentModel.DataAnnotations;

namespace Prototype_Backend.Models
{
    public class Identity
    {
        [Key]
        public int ID { get; set; }
        public string UserEmail { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
    }
}
