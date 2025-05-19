using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;


namespace WH.Models
{
    [Table("dmkh", Schema = "public")]
    public class DmkhClass
    {
        [Key]
        [Column("ma_kh")]
        [StringLength(35)]
        public string ma_kh { get; set; }

        [Column("ten_kh")]
        [StringLength(100)]
        public string ten_kh { get; set; }

        [Column("dia_chi")]
        public string dia_chi { get; set; }

        [Column("dien_thoai")]
        [StringLength(12)]
        public string dien_thoai { get; set; }

        [Column("mo_ta")]
        public string mo_ta { get; set; }
        [Column("ma_so_thue")]
        public string ma_so_thue { get; set; }
    }
}