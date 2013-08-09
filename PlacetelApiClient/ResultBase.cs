using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlacetelApiClient
{
    public class ResultBase
    {
        public int Result { get; set; }
        public string ResultCode { get; set; }
        public bool Success { get { return Result == 1; } }
    }
}
