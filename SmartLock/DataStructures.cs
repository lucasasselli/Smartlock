using System;
using Microsoft.SPOT;

namespace SmartLock
{
    [Serializable]
    public class UserForLock
    {
        public string Pin { get; set; }
        public string CardID { get; set; }
        public string Expire { get; set; }
    }

    public class Log
    {
        public Log(int type, string pin, string cardID, string text, string dateTime) //type 1
        {
            Type = type;
            Pin = pin;
            CardID = cardID;
            Text = text;
            DateTime = dateTime;
        }

        public Log(int type, string text, string dateTime) //type 2
        {
            Type = type;
            Text = text;
            DateTime = dateTime;
        }

        public Log(int type, string pin, string text, string dateTime) //type 4
        {
            Type = type;
            Pin = pin;
            Text = text;
            DateTime = dateTime;
        }

        public int Type { get; set; }
        public string Pin { get; set; }
        public string CardID { get; set; }
        public string Text { get; set; }
        public string DateTime { get; set; }
    } //end class
} //end namespace
