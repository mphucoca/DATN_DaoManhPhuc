using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
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
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(bytes);

                // Convert to hex string
                StringBuilder builder = new StringBuilder();
                foreach (var b in hashBytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
    }
}
