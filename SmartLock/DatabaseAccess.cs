using System;
using System.IO;
using System.Collections;
using System.Threading;
using System.Net;

using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

namespace SmartLock
{
    // This will most likely substitute ServerAccess
    public class DatabaseAccess
    {
        // Request + JSON stuff
        private const string ServerIP = "192.168.1.101";
        private const string ServerPort = "8000";
        private const string GadgeteerID = "1";
        private const string URL = "http://" + ServerIP + ":" + ServerPort + "/SmartLockRESTService/data/?id=" + GadgeteerID; 
        private const string UserHeader = "AllowedUsers";
        private static string[] fields_user_name = { "CardID", "Expire", "Pin" };
        private const string LogHeader = "Log";
        private static string[] fields_log_name = { "Type", "Pin", "CardID", "Text", "DateTime" };

        // Loads userlist from db
        public bool RequestUsers(ArrayList userList)
        {
            bool isOk = false; 

            Debug.Print("Sending request to Server...");

            HttpWebRequest request = WebRequest.Create(URL) as HttpWebRequest;
            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Stream response_stream = response.GetResponseStream();
                        StreamReader response_reader = new StreamReader(response_stream);
                        string response_string = response_reader.ReadToEnd();
                        response_stream.Close();
                        response_reader.Close();
                        parseJsonResponse(response_string, userList);

                        isOk = true;
                    }

                    Debug.Print("Response is: " + response.StatusCode.ToString());
                }
            }
            catch (Exception e)
            {
                Debug.Print("ERROR: Exception while getting user list: " + e.ToString());
                return false;
            }

            return isOk;
        }

        // Sends log to db
        public bool SendLogs(ArrayList logList)
        {
            string json_string = builtJsonLogs(logList);
            HttpWebRequest request = WebRequest.Create(URL) as HttpWebRequest;
            request.ContentType = "application/json; charset=utf-8";
            request.Method = "POST";
            try
            {
                Stream request_stream = request.GetRequestStream();
                StreamWriter request_writer = new StreamWriter(request_stream);
                request_writer.Write(json_string);
                request_stream.Close();
                request_writer.Close();
            }catch (Exception e){
                Debug.Print("ERROR: Exception while sending logs: " + e.ToString());
                return false;
            }

            return true;
        }

        // Json parser (Userlist)
        private void parseJsonResponse(string response_string, ArrayList destList)
        {
            int start_header = response_string.IndexOf('\"');
            int end_header = response_string.IndexOf('\"', start_header + 1);
            string header = response_string.Substring(start_header + 1, end_header - 2);
            if (UserHeader.Equals(header))
            {
                string json = response_string.Substring(end_header + 3,
                    response_string.Length - end_header - 5);
                //here I have: {"CardID":"ABCDE","Expire":"31\/03\/2017 12:46:59","Pin":"12345"},{"CardID":null,"Expire":"01\/04\/2017 12:46:59","Pin":"67891"}
                Debug.Print(json);
                string[] AllowedUsers_string = json.Split('}'); //divide users
                int UsersNumbers = AllowedUsers_string.Length - 1; //number of users
                for (int i = 0; i < UsersNumbers; i++) //for every user
                {
                    UserForLock User = new UserForLock();
                    AllowedUsers_string[i] = AllowedUsers_string[i] + '}'; //correct the string
                    char[] SingleUser = AllowedUsers_string[i].ToCharArray(); //convert to char array
                    string field;
                    string field_value;
                    int j = 0;
                    while (SingleUser[j] != '}') //till the end of the string
                    {
                        field = "";
                        field_value = "";
                        while (SingleUser[++j] != '\"') ;
                        while (SingleUser[++j] != '\"')
                            field = field + SingleUser[j]; //obtain the field name
                        Debug.Print(field);
                        j = j + 2; //skip , and "
                        if (SingleUser[j] == '\"')
                        {
                            while (SingleUser[++j] != '\"')
                                field_value = field_value + SingleUser[j]; //obtain field value
                            if (SingleUser[j] == '}') //if string end
                            {
                                writeList(i, field, field_value, User);
                                Debug.Print(field_value);
                                break; //exit
                            }
                            j++; //skip ,
                        }
                        else //null field
                        {
                            field_value = field_value + SingleUser[j];
                            while (SingleUser[++j] != ',' && SingleUser[j] != '}')
                                field_value = field_value + SingleUser[j]; //obtain field value
                        }
                        writeList(i, field, field_value, User);
                        Debug.Print(field_value);
                    }
                    Debug.Print("User CardID: " + User.CardID);
                    Debug.Print("USer Expire: " + User.Expire);
                    Debug.Print("USer Pin: " + User.Pin);
                    destList.Add(User);
                }
            }
            else
            {
                Debug.Print("Data not recognized");
                //TODO
            }
        }

        // Json builder (Log)
        private static string builtJsonLogs(ArrayList Logs) //can send multiple logs at a time
        {
            string json_string = "{\"" + LogHeader + "\":[";
            foreach (Log log in Logs)
            {
                json_string = json_string + "{\"" + fields_log_name[0] + "\":";
                switch (log.Type)
                {
                    case 1:
                        json_string = json_string + log.Type.ToString() + ",\"" +
                            fields_log_name[1] + "\":\"" + log.Pin + "\",\"" +
                            fields_log_name[2] + "\":\"" + log.CardID + "\",\"" +
                            fields_log_name[3] + "\":\"" + log.Text + "\",\"" +
                            fields_log_name[4] + "\":\"" + log.DateTime;
                        break;
                    case 2:
                        json_string = json_string + log.Type.ToString() + ",\"" +
                            fields_log_name[3] + "\":\"" + log.Text + "\",\"" +
                            fields_log_name[4] + "\":\"" + log.DateTime;
                        break;
                    case 4:
                        json_string = json_string + log.Type.ToString() + ",\"" +
                            fields_log_name[1] + "\":\"" + log.Pin + "\",\"" +
                            fields_log_name[3] + "\":\"" + log.Text + "\",\"" +
                            fields_log_name[4] + "\":\"" + log.DateTime;
                        break;
                }
                json_string = json_string + "\"},";
            }
            json_string = json_string.Substring(0, json_string.Length - 1); //remove "," of last log
            json_string = json_string + "]}";
            return json_string;
        }

        // Updates single use json
        private void writeList(int i, string field, string field_value, UserForLock User)
        {
            if (field_value.Equals("null"))
                field_value = null;
            if (field.Equals(fields_user_name[0]))
                User.CardID = field_value;
            else if (field.Equals(fields_user_name[1]))
                User.Expire = field_value;
            else if (field.Equals(fields_user_name[2]))
                User.Pin = field_value;
        }
    }
} 
