using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    [Table("ProductTypes")]
    public class ProductTypes
    {
        
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Product Type")]
        public string ProductType { get; set; }

    }
}
