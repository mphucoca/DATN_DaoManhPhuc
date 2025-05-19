using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;


namespace WH.Models
{
    [Table("dmqddvt", Schema = "public")]
    public class DmqddvtClass
    {
        [Key]
        [Column("ma_vt", Order = 0)]
        [StringLength(35)]
        [DisplayName("Mã vật tư")]
        public string ma_vt { get; set; }

        [Key]
        [Column("ma_dvt", Order = 1)]
        [StringLength(35)]
        [DisplayName("Mã đơn vị tính")]
        public string ma_dvt { get; set; }

        [Column("ty_le_quy_doi")]
        public decimal? ty_le_quy_doi { get; set; }

    }
    public class DmdvtQddvtViewModel
    {
        [DisplayName("Mã vật tư")]
        public string ma_vt { get; set; }

        [DisplayName("Mã đơn vị tính")]
        public string ma_dvt { get; set; }

        [DisplayName("Tên đơn vị tính")]
        public string ten_dvt { get; set; }

        [DisplayName("Mô tả")]
        public string mo_ta { get; set; }

        [DisplayName("Trạng thái")]
        public int? trangthai { get; set; }

        [DisplayName("Tỷ lệ quy đổi")]
        public decimal? ty_le_quy_doi { get; set; }
    }
}
