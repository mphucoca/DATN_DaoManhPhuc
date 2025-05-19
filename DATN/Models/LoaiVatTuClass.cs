using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
namespace WH.Models
{
    [Table("loai_vat_tu", Schema = "public")]
    public class LoaiVatTuClass
    {
        [Key]
        [Column("ma_loai_vt")]
        [StringLength(35)]
        [DisplayName("Mã loại vật tư")]
        public string ma_loai_vt { get; set; }

        [Column("ten_loai_vt")]
        [StringLength(100)]
        [DisplayName("Tên loại vật tư")]
        public string ten_loai_vt { get; set; }

        [Column("mo_ta")]
        [DisplayName("Mô tả")]
        public string mo_ta { get; set; }
        [Column("trangthai")]
        [DisplayName("Trạng thái")]
        public int trangthai { get; set; }
    }

}