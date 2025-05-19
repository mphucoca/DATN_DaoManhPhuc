using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace WH.Models
{
    [Table("dmdvt", Schema = "public")]
    public class DmdvtClass
    {
        [Key]
        [Column("ma_dvt")]
        [StringLength(35)]
        [DisplayName("Mã đơn vị tính")]
        public string ma_dvt { get; set; }

        [Column("ten_dvt")]
        [StringLength(100)]
        [DisplayName("Tên đơn vị tính")]
        public string ten_dvt { get; set; }

        [Column("mo_ta")]
        [DisplayName("Mô tả")]
        public string mo_ta { get; set; }
 
        [Column("trangthai")]
        [DisplayName("Trạng thái")]
        public int trangthai { get; set; }
    }
}