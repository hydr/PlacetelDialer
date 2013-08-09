using System;

namespace PlacetelApiClient
{
    public class Call
    {
        public int CallType { get; set; } 
        public string FromNumber { get; set; }
        public string ToNumber { get; set; }
        public DateTime ReceivedAt { get; set; }
        public string FileName { get; set; }
    }
}