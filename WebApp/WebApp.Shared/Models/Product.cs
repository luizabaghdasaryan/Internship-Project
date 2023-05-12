using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Shared.Models
{
    public class Product : ICloneable
    {
        public int ID { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Brand { get; set; }
        [Required]
        public string Category { get; set; } = "Face";
        public override bool Equals(object? obj)
        {
            if (obj is Product product) return ID == product.ID;
            return false;
        }
        public override int GetHashCode() => Name.GetHashCode();
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}