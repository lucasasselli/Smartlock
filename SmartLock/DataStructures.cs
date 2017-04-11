using System;
using Microsoft.SPOT;

namespace SmartLock
{
    /*
     * NOTE: Serializizable class attribute allows object serialization!
     * 
     * DO NOT REMOVE OR EDIT!
     */

    [Serializable]
    public class UserForLock
    {
        public string Pin { get; set; }
        public string CardID { get; set; }
        public string Expire { get; set; }
    }

    [Serializable]
    public class Log
    {
        public const int TYPE_ACCESS = 1;
        public const int TYPE_INFO = 2;
        public const int TYPE_ERROR = 4;

        public Log(int type, string pin, string cardID, string text, string dateTime) 
        {
            Type = type;
            Pin = pin;
            CardID = cardID;
            Text = text;
            DateTime = dateTime;
        }

        public Log(int type, string text, string dateTime)
        {
            Type = type;
            Text = text;
            DateTime = dateTime;
        }

        public Log(int type, string pin, string text, string dateTime)
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
    } 
}
