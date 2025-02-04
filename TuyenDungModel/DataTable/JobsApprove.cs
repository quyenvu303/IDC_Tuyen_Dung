using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace TuyenDungModel.DataTable
{
    public partial class JobsApprove
    {
        public int Id { get; set; }
        public int? JobsId { get; set; }
        public int? UserId { get; set; }
        public int? StepApprove { get; set; }
        public string Comment { get; set; }
        public DateTime? DateCreate { get; set; }
        public int? Status { get; set; }
    }
}
