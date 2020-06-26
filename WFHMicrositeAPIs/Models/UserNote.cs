using System;
using System.Collections.Generic;

namespace WFHMicrositeAPIs.Models
{
    public partial class UserNote
    {
        public int UserNoteId { get; set; }
        public int UserId { get; set; }
        public string Csuser { get; set; }
        public string Note { get; set; }
        public DateTime Date { get; set; }
    }
}
