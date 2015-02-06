using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Questionnaire2.Models
{
    public class AppSetting
    {
        public int AppSettingId { get; set; }

        [Display(Name = "Setting Name")]
        public string AppSettingName { get; set; }

        [Display(Name = "Setting Value")]
        [UIHint("tinymce_jquery_full"), AllowHtml]
        public string AppSettingValue { get; set; }
    }
}