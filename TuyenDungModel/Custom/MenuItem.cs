using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TuyenDungModel.Custom
{
    public partial class MenuItem
    {
        [Key]
        public int id { get; set; } // Ánh xạ với cột Id
        public int? parent_id { get; set; } // Ánh xạ với cột Parent_Id
        public string Menu_name { get; set; } // Ánh xạ với cột MenuName
        public string icon { get; set; } // Ánh xạ với cột Icon
        public string sort_order { get; set; } // Ánh xạ với cột Sort_Order
        public string Action { get; set; } // Ánh xạ với cột Action
        public string Controller { get; set; } // Ánh xạ với cột Url
        public string Areas { get; set; }
        [NotMapped] // Bỏ qua ánh xạ với cơ sở dữ liệu
        public List<MenuItem> Children { get; set; } = new List<MenuItem>();
    }

}
