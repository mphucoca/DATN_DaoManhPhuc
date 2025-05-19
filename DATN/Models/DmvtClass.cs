using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace WH.Models
{
    [Table("dmvt", Schema = "public")]
    public class DmvtClass
    {
        [Key]
        [Column("ma_vt")]
        [StringLength(35)]
        [DisplayName("Mã vật tư")]
        public string ma_vt { get; set; }

        [Column("ma_loai_vt")]
        [StringLength(35)]
        [DisplayName("Mã loại vật tư")]
        public string ma_loai_vt { get; set; }

        [Column("ten_vt")]
        [StringLength(100)]
        [DisplayName("Tên vật tư")]
        public string ten_vt { get; set; }

        [Column("min_ton")]
        public int? min_ton { get; set; }

        [Column("max_ton")]
        public int? max_ton { get; set; }

        [Column("barcode")]
        [StringLength(50)]
        public string barcode { get; set; }

        [Column("url")]
        [StringLength(100)]
        public string url { get; set; }

    

        [Column("rong")]
        public decimal? rong { get; set; }

        [Column("cao")]
        public decimal? cao { get; set; }

        [Column("khoi_luong")]
        public decimal? khoi_luong { get; set; }

        [Column("mau_sac")]
        public string mau_sac { get; set; }

        [Column("kieu_dang")]
        public string kieu_dang { get; set; }
        [Column("trangthai")]
        public int trangthai { get; set; }
        [Column("mo_ta")]
        public string mo_ta { get; set; }
        [Column("ma_dvt" )]
        [StringLength(35)]
        [DisplayName("Mã đơn vị tính chuẩn")]
        public string ma_dvt { get; set; }

    }
}