using System;

namespace PlacetelApiClient
{
    public class CallDetailRecord
    {
        public decimal Amount { get; set; }
        public string Descr { get; set; }
        public string From { get; set; }
        public int Length { get; set; }
        public string To { get; set; }
        public DateTime WhenDate { get; set; }
    }
}