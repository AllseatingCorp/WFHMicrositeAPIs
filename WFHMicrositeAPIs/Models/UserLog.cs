﻿using System;
using System.Collections.Generic;

#nullable disable

namespace WFHMicrositeAPIs.Models
{
    public partial class UserLog
    {
        public int UserLogId { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; }
        public string Details { get; set; }
        public DateTime Updated { get; set; }
    }
}
