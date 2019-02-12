using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CropImage.Models.SysTem
{
    public class LogInfo
    {
        [Key]
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string EntityName { get; set; }
        public string Action { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime TimeOccur { get; set; }
        public string Descript { get; set; }
        public DateTime CreateDate { get; set; }
        public LogInfo()
        {
            this.CreateDate = DateTime.Now;
        }
    }
    
}