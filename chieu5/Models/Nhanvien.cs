namespace chieu5.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Nhanvien")]
    public partial class Nhanvien
    {
        [Key]
        [StringLength(6)]
        public string MaNV { get; set; }

        [Required]
        [StringLength(20)]
        public string TenNV { get; set; }

        [Required]
        [StringLength(2)]
        public string MaPB { get; set; }

        [Column(TypeName = "date")]
        public DateTime Ngaysinh { get; set; }

        public virtual Phongban Phongban { get; set; }
    }
}
