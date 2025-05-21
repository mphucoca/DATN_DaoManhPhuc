using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WH.Models
{
    public class BCPhieuXuat
    {
        public double stt { get; set; }         
        public double sys_order { get; set; }
        public double sys_print { get; set; }
        public double sys_total { get; set; }
        public string so_ct { get; set; }
        public DateTime ngay_ct { get; set; }
        public string ma_kho { get; set; }
        public string ten_kho { get; set; }
        public string ma_kh { get; set; }
        public string ten_kh { get; set; }
        public string ma_vt { get; set; }
        public string ten_vt { get; set; }
        public string ma_dvt { get; set; }
        public string ten_dvt { get; set; }
        public double so_luong_xuat { get; set; }   
        public double don_gia_xuat { get; set; }     
        public double thanh_tien { get; set; }       
    }
}