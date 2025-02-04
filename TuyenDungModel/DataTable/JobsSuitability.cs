using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace TuyenDungModel.DataTable
{
    public partial class JobsSuitability
    {
        public int Id { get; set; }
        public int? JobId { get; set; }
        public int? DoPhuHopId { get; set; }
        public int? BatBuoc { get; set; }
        public int? QuanTrong { get; set; }
        public int? ItQuanTrong { get; set; }
        public int? Status { get; set; }
    }
}
