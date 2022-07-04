using System;
using System.ComponentModel.DataAnnotations;

namespace Prototype_Backend.Models
{
    public class User
    {
        // Är det bättre att generera en Guid för Key och helt enkelt tagga Email och Username som [Index(isUnique=true)] med [StringLength(450)]?
        [Key]
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string Username { get; set; }

        public string ProfileUri { get; set; }
        public string PhoneNumber { get; set; }
    }
}
