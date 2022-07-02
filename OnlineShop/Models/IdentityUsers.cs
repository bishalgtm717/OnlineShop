 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    [Table("AspNetUsers")]
    public class IdentityUsers
    {
        [Key]
        public string Id { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumber { get; set; }
        public bool FirstName { get; set; }
        public bool LastName { get; set; }
    }
}
