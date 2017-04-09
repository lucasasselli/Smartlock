using System;
using System.IO;
using System.Collections;
using System.Threading;
using System.Net;
using System.Text;

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

        // Loads userlist from db
        public bool RequestUsers(ArrayList userList)
        {
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
                        if (!Json.ParseNamedArray(UserHeader, response_string, userList, typeof(UserForLock)))
                        {
                            return false;
                        }
                    }

                    Debug.Print("Response is: " + response.StatusCode.ToString());
                }
            }
            catch (Exception e)
            {
                Debug.Print("ERROR: Exception while getting user list: " + e.ToString());
                return false;
            }

            return true;
        }

        // Sends log to db
        public bool SendLogs(ArrayList logList)
        {
            string jsonString = Json.BuildNamedArray(LogHeader, logList);
            HttpWebRequest request = WebRequest.Create(URL) as HttpWebRequest;
            byte[] requestByteArray = Encoding.UTF8.GetBytes(jsonString);

            try
            {
                // Send the request
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = requestByteArray.Length;

                Stream postStream = request.GetRequestStream();

                postStream.Write(requestByteArray, 0, requestByteArray.Length);
                postStream.Close();

                // Grab the response
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    String responseValue = string.Empty;

                    // Error
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Debug.Print("ERROR: request failed. Received HTTP " + response.StatusCode);
                        return false;
                    }

                    // Grab the response
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            using (var reader = new StreamReader(responseStream))
                            {
                                responseValue = reader.ReadToEnd();
                            }
                        }
                    }

                    // Print response for debug purposes
                    Debug.Print("Server response: " + responseValue);
                }

            }
            catch (Exception e)
            {
                Debug.Print("ERROR: Exception while sending logs: " + e.ToString());
                return false;
            }

            return true;
        }
    }
} 
