using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace TuyenDungModel.DataTable
{
    public partial class ManagementSettingRecruitment
    {
        public int Id { get; set; }
        public string StepName { get; set; }
        public int? StepOrder { get; set; }
        public int? Status { get; set; }
    }
}
