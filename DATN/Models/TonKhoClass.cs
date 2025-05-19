using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;


namespace WH.Models
{
    [Table("tonkho", Schema = "public")]
    public class TonKhoClass
    {
        [Key]
        [Column("ma_vt" )]
        [StringLength(35)]
        public string ma_vt { get; set; }

     
        [Column("ma_kho", Order = 2)]
        [StringLength(35)]
        public string ma_kho { get; set; }
 
        [Column("ma_dvt", Order = 3)]
        [StringLength(35)]
        public string ma_dvt { get; set; }

        [Column("so_luong_ton")]
        public int so_luong_ton { get; set; }

        [Column("ngay_cap_nhat")]
        public DateTime? ngay_cap_nhat { get; set; }
    }
    public class TonKhoViewModel
    {
        public string ma_vt { get; set; }
        public string ma_kho { get; set; }
        public string ma_dvt { get; set; }
        public string ten_dvt { get; set; }
        public string mo_ta { get; set; }
        public int trangthai { get; set; }
        public int so_luong_ton { get; set; }
        public DateTime? ngay_cap_nhat { get; set; }
    }
    public class BCTonKho
    {
        public double? stt { get; set; }
        public double? sys_order { get; set; }
        public double? sys_print { get; set; }
        public double? sys_total { get; set; }

        public string ma_kho { get; set; }
        public string ten_kho { get; set; }
        public string mo_ta_kho { get; set; }
        public string dia_chi_kho { get; set; }

        public string ma_vt { get; set; }
        public string ten_vt { get; set; }
        public double? rong { get; set; }
        public double? cao { get; set; }
        public double? khoi_luong { get; set; }
        public string mau_sac { get; set; }
        public string kieu_dang { get; set; }

        public string ma_dvt { get; set; }
        public string ten_dvt { get; set; }
        public string mo_ta_dvt { get; set; }

        public double? ty_le_quy_doi { get; set; }
        public double? so_luong_ton { get; set; }
        public System.DateTime? ngay_cap_nhat { get; set; }
    }



}