
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    [Table("UserRoles")]
    public class UserRoles
    {
        [Key]
        public string UserId { get; set; }
        public string RoleId { get; set; }
    }
}
