using System;
using Microsoft.SPOT;

namespace SmartLock
{
    public partial class Program
    {
        // Tag Found event
        void TagFound(string uid)
        {
            bool authorized = dataHelper.CheckCardID(uid);
            ParseAccess(authorized);
        }
    }
}
