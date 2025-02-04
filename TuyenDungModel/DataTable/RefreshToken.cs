using System;
using System.ComponentModel.DataAnnotations.Schema;
using TuyenDungModel.DataTable;

namespace TuyenDungModel.DataTable
{
    public partial class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string CreatedBy { get; set; }
        public bool? Revoked { get; set; }
        public DateTime? RevokedAt { get; set; }
        [NotMapped]
        public virtual ManagementUser User { get; set; }
    }
}
