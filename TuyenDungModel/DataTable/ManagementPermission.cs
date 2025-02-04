using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace TuyenDungModel.DataTable
{
    public partial class ManagementPermission
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? MenuId { get; set; }
        public int? FullControl { get; set; }
        public int? Access { get; set; }
        public int? View { get; set; }
        public int? Edit { get; set; }
        public int? Insert { get; set; }
        public int? Delete { get; set; }
        public int? Status { get; set; }
    }
}
