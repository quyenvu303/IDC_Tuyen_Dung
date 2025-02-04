using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace TuyenDungModel.DataTable
{
    public partial class ManagementWebsiteContent
    {
        public int Id { get; set; }
        public string ContentVi { get; set; }
        public string ContentEn { get; set; }
        public int? Status { get; set; }
    }
}
