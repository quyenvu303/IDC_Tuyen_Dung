using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TuyenDungModel.Custom
{
    public partial class EmployeesViewLogin
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int? JobTitleId { get; set; }
        public string JobTitleName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public string Avatar { get; set; }
        public int IsActive { get; set; }
        public int Status { get; set; }
    }
    public class UserNoResult
    {
        [Key]
        public string NextUserNo { get; set; }
    }
}
