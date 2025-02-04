using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace TuyenDungModel.DataTable
{
    public partial class ManagementLocationWork
    {
        public int Id { get; set; }
        public string OfficeNameVi { get; set; }
        public string OfficeNameEn { get; set; }
        public string LocationName { get; set; }
        public string DistrictCity { get; set; }
        public int? Status { get; set; }
    }
}
