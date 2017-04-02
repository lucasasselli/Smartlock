using System;
using Microsoft.SPOT;

namespace SmartLock
{
    public class UserForLock
    {
        public string Pin { get; set; }
        public string CardID { get; set; }
        public string Expire { get; set; }
    }
}
