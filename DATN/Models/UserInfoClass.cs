using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace WH.Models
{
    [Table("userinfo", Schema = "public")]
    public class UserInfoClass
    {
        [Key]
        [Column("id")]
        [StringLength(35)]
        [DisplayName("Mã người dùng")]
        public string id { get; set; }

        [Column("username")]
        [StringLength(35)]
        [DisplayName("Tên đăng nhập")]
        public string username { get; set; }

        [Column("fullname")]
        [StringLength(100)]
        [DisplayName("Tên đầy đủ")]
        public string fullname { get; set; }

        [Column("password")]
        [StringLength(35)]
        [DisplayName("Mật khẩu")]
        public string password { get; set; }
        [Column("email")]
        [StringLength(50)]
        [DisplayName("email")]
        public string email { get; set; }
        [Column("role")]
        [DisplayName("Vai trò")]
        public int role { get; set; }
        [Column("trangthai")]
        [DisplayName("Trạng thái")]
        public int trangthai { get; set; }
    }
}
