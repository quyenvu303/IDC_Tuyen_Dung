using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TuyenDungModel.Custom
{
    public class ManagementDepartment_LoadDepartment
    {
        [Key]
        public int Id { get; set; }
        public int? ParenId { get; set; }
        public string department_code { get; set; }
        public string department_name { get; set; }
    }
}
