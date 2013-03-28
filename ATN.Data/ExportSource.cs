using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATN.Data
{
    public class ExportSource
    {
        public long SourceId { get; set; }
        public string MasID { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public string Authors { get; set; }
        public string Journal { get; set; }
    }
}
