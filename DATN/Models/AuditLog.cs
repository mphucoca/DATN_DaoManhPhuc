using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WH.Models
{
    [Table("audit_log", Schema = "public")]
    public class AuditLog
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("table_name")]
        [Required]
        [StringLength(100)]
        public string table_name { get; set; }

        [Column("operation")]
        [Required]
        [StringLength(10)]
        public string operation { get; set; } // INSERT, UPDATE, DELETE

        [Column("primary_key_data")]
        public string primary_key_data { get; set; } // JSON

        [Column("old_data")]
        public string old_data { get; set; } // JSON

        [Column("new_data")]
        public string new_data { get; set; } // JSON

        [Column("changed_by")]
        [StringLength(100)]
        public string changed_by { get; set; }

        [Column("changed_at")]
        public DateTime changed_at { get; set; }
    }
    public class AuditLogView
    { 
        public int id { get; set; } 
        public string table_name { get; set; } 
        public string operation { get; set; }  
        public string primary_key_data { get; set; }  
        public string old_data { get; set; }  
        public string new_data { get; set; }  
        public string changed_by { get; set; } 
        public DateTime changed_at { get; set; }
        public string username { set; get; }
    }
}
