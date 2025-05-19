using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;


namespace WH.Models
{
    [Table("dmncc", Schema = "public")]
    public class DmnccClass
    {
        [Key]
        [Column("ma_ncc")]
        [StringLength(35)]
        [DisplayName("Mã nhà cung cấp")]
        public string ma_ncc { get; set; }

        [Column("ten_ncc")]
        public string ten_ncc { get; set; }

        [Column("dia_chi")]
        public string dia_chi { get; set; }

        [Column("ghi_chu")]
        public string ghi_chu { get; set; }

        [Column("dien_thoai")]
        [StringLength(11)]
        public string dien_thoai { get; set; }

        [Column("email")]
        [StringLength(50)]
        public string email { get; set; }
        [Column("ma_so_thue")]
        [StringLength(50)]
        public string ma_so_thue { get; set; }
    }
}