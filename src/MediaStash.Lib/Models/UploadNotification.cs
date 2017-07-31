using System;
using System.Collections.Generic;
using System.Text;

namespace MediaStash.Lib.Models
{
    public class UploadNotification
    {
        public int TotalFiles { get; set; }
        public double TotalMegabytes { get; set; }
        public double ProcessedMegabytes { get; set; }
    }
}
