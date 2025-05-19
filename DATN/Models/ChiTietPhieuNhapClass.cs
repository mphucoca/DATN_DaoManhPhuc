using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;


namespace WH.Models
{
    [Table("ct_phieu_nhap", Schema = "public")]
    public class ChiTietPhieuNhapClass
    {
        [Key, Column("ma_vt", Order = 0)]
        [StringLength(35)]
        public string ma_vt { get; set; }

        [Key, Column("ma_ncc", Order = 1)]
        [StringLength(35)]
        public string ma_ncc { get; set; }

        [Key, Column("ma_kho", Order = 2)]
        [StringLength(35)]
        public string ma_kho { get; set; }

        [Key, Column("so_ct", Order = 3)]
        [StringLength(35)]
        public string so_ct { get; set; }

        [Key, Column("ma_dvt", Order = 4)]
        [StringLength(35)]
        public string ma_dvt { get; set; }

        [Column("so_luong_nhap")]
        public int so_luong_nhap { get; set; }

        [Column("don_gia_nhap")]
        public decimal don_gia_nhap { get; set; }

        [Column("ghi_chu")]
        public string ghi_chu { get; set; }

    }
}