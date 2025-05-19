using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace WH.Models
{
    [Table("ct_phieu_xuat", Schema = "public")]
    public class ChiTietPhieuXuatClass
    {
        [Key]
        [Column("ma_vt" )]
        [StringLength(35)]
        public string ma_vt { get; set; }

   
        [Column("ma_kho" )]
        [StringLength(35)]
        public string ma_kho { get; set; }

      
        [Column("so_ct" )]
        [StringLength(35)]
        public string so_ct { get; set; }

   
        [Column("ma_kh" )]
        [StringLength(35)]
        public string ma_kh { get; set; }

     
        [Column("ma_dvt" )]
        [StringLength(35)]
        public string ma_dvt { get; set; }

        [Column("so_luong_xuat")]
        public int so_luong_xuat { get; set; }

        [Column("don_gia_xuat")]
        public decimal don_gia_xuat { get; set; }

        [Column("ghi_chu")]
        public string ghi_chu { get; set; }

    }
    public class ChiTietPhieuXuatViewClass
    {
        
        public string ma_vt { get; set; } 
        public string ma_kho { get; set; } 
        public string so_ct { get; set; } 
        public string ma_kh { get; set; } 
        public string ma_dvt { get; set; } 
        public int so_luong_xuat { get; set; } 
        public decimal don_gia_xuat { get; set; } 
        public string ghi_chu { get; set; }
        public decimal ton { set; get; }

    }
}