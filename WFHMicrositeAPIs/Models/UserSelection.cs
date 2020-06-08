using System;
using System.Collections.Generic;

namespace WFHMicrositeAPIs.Models
{
    public partial class UserSelection
    {
        public int UserSelectionId { get; set; }
        public int UserId { get; set; }
        public int ProductOptionId { get; set; }
        public string Type { get; set; }
    }
}
