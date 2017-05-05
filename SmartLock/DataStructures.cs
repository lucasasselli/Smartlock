using System;
using GHI.Processor;
using DateTimeExtension;

namespace SmartLock
{
    /*
     * NOTE: Serializizable class attribute allows object serialization!
     * DO NOT REMOVE OR EDIT!
     */

    [Serializable]
    public class User
    {
        public string Pin { get; set; }
        public string CardID { get; set; }
        public string Expire { get; set; }
    }

    [Serializable]
    public class Log
    {
        // Types
        public const int TypeAccess = 1;
        public const int TypeInfo = 2;
        public const int TypeError = 4;

        // Fields
        public int Type { get; set; }
        public string Pin { get; set; }
        public string CardID { get; set; }
        public string Text { get; set; }
        public string DateTime { get; set; }

        public Log(int type, string pin, string cardId, string text)
        {
            Type = type;
            Pin = pin;
            CardID = cardId;
            Text = text;
            DateTime = RealTimeClock.GetDateTime().ToMyString();
        }

        public Log(int type, string text)
        {
            Type = type;
            Text = text;
            DateTime = RealTimeClock.GetDateTime().ToMyString();
        }

        public Log(int type, string pin, string text)
        {
            Type = type;
            Pin = pin;
            Text = text;
            DateTime = RealTimeClock.GetDateTime().ToMyString();
        }
    } 
}
