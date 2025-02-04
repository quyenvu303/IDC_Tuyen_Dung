using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TuyenDungModel.Custom
{
    public class ManagementMenu_LoadMenu
    {
        [Key]
        public int Id { get; set; }
        public int? ParenId { get; set; }
        public string MenuName { get; set; }
    }
}
