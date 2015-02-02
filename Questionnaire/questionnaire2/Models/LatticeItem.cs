using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Questionnaire2.Models
{
    public class LatticeItem
    {
        [Key]
        public int ItemId { get; set; }
        public string Step { get; set; }
        public string LatticeLevel { get; set; }
        public string ItemType { get; set; }
        public string ItemName { get; set; }
        public string DropdownText { get; set; }
    }
}