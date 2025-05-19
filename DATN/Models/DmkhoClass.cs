using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;


namespace WH.Models
{
    [Table("dmkho", Schema = "public")]
    public class DmkhoClass
    {
        [Key]
        [Column("ma_kho")]
        [StringLength(35)]
        public string ma_kho { get; set; }

        [Column("ten_kho")]
        [StringLength(100)]
        public string ten_kho { get; set; }

        [Column("mo_ta")]
        public string mo_ta { get; set; }

        [Column("dia_chi")]
        public string dia_chi { get; set; }
    }
}