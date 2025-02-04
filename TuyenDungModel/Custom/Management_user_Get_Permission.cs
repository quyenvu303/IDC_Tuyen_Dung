using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TuyenDungModel.Custom
{
    public class Management_user_Get_Permission
    {
        [Key]
        public int id { get; set; }
        public string controller { get; set; }
        public int? full_control { get; set; }
        public int? access { get; set; }
        public int? view { get; set; }
        public int? insert { get; set; }
        public int? edit { get; set; }
        public int? delete { get; set; }
        public int? MenuId { get; set; }
    }
}
