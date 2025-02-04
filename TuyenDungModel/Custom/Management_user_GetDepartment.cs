using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TuyenDungModel.Custom
{
    public class Management_user_GetDepartment
    {
        [Key]
        public int Id { get; set; }
        public int? ParenId { get; set; }
        public string DepartmentName { get; set; }
    }
}
