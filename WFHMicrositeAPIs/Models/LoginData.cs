using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WFHMicrositeAPIs.Models
{
    public class LoginData
    {
        public string EmailAddress { get; set; }
        public string PIN { get; set; }
        public int? UserId { get; set; }
    }
}
