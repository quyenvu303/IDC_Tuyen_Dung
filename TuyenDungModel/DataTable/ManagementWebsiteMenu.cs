using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace TuyenDungModel.DataTable
{
    public partial class ManagementWebsiteMenu
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Banner { get; set; }
        public string MenuNameVi { get; set; }
        public string MenuNameEn { get; set; }
        public int? Status { get; set; }
    }
}
