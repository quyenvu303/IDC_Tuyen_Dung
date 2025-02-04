using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace TuyenDungModel.DataTable
{
    public partial class ManagementSocial
    {
        public int Id { get; set; }
        public string SocialName { get; set; }
        public string Icon { get; set; }
        public string LinkSocial { get; set; }
        public int? Status { get; set; }
    }
}
