using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;


namespace WH.Models
{
    [Table("phieu_nhap", Schema = "public")]
    public class PhieuNhapClass
    {
        [Key]
        [Column("so_ct")]
        [StringLength(35)]
        public string so_ct { get; set; }

        [Column("ma_ncc")]
        [StringLength(35)]
        public string ma_ncc { get; set; }

        [Column("ngay_ct")]
        public DateTime? ngay_ct { get; set; }

        [Column("dien_giai")]
        public string dien_giai { get; set; }

        [Column("trang_thai")]
        public int? trang_thai { get; set; }

        [Column("bien_so_xe")]
        public string bien_so_xe { get; set; }

        [Column("ngay_van_chuyen")]
        public DateTime? ngay_van_chuyen { get; set; }

        [Column("tt_thanhtoan")]
        public int? tt_thanhtoan { get; set; }

        [Column("chietkhau")]
        public int? chietkhau { get; set; }

        [Column("ma_so_thue")]
        public string ma_so_thue { get; set; }

        [Column("thue")]
        public int? thue { get; set; }
        [Column("tong_thanh_toan")]
        public decimal? tong_thanh_toan { get; set; }

        [Column("tong_chiet_khau")]
        public decimal? tong_chiet_khau { get; set; }

        [Column("tong_thue")]
        public decimal? tong_thue { get; set; }
    }
    public class PhieuNhapViewModel
    {
        public string so_ct { get; set; }
        public string ma_ncc { get; set; }
        public DateTime? ngay_ct { get; set; }
        public string dien_giai { get; set; }
        public int? trang_thai { get; set; }
        public string ten_ncc { get; set; } // tên nhà cung cấp

        public string bien_so_xe { get; set; }
        public DateTime? ngay_van_chuyen { get; set; }
        public int? tt_thanhtoan { get; set; }
        public int? chietkhau { get; set; }
        public string ma_so_thue { get; set; }
        public int? thue { get; set; }
        public decimal? tong_thanh_toan { get; set; }
        public decimal? tong_chiet_khau { get; set; }
        public decimal? tong_thue { get; set; }

    }
}