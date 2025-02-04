﻿using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace TuyenDungModel.DataTable
{
    public partial class JobsConfigPhanLai
    {
        public int Id { get; set; }
        public int? JobId { get; set; }
        public int? Step { get; set; }
        public string StepName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int? Status { get; set; }
    }
}
