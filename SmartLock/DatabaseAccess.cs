using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.SPOT;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;

namespace SmartLock
{
    public class DatabaseAccess
    {
        // Request + JSON stuff
        private const string UserHeader = "AllowedUsers";
        private const string LogHeader = "Log";

        // Loads userlist from db
        public bool RequestUsers(ArrayList userList)
        {
            string url = buildUrlFromSettings();
            var request = WebRequest.Create(url) as HttpWebRequest;
            try
            {
                if (request != null)
                    using (var response = request.GetResponse() as HttpWebResponse)
                    {
                        if (response != null && response.StatusCode == HttpStatusCode.OK)
                        {
                            var responseStream = response.GetResponseStream();
                            var responseReader = new StreamReader(responseStream);
                            var responseString = responseReader.ReadToEnd();
                            responseStream.Close();
                            responseReader.Close();
                            if (!Json.ParseNamedArray(UserHeader, responseString, userList, typeof(UserForLock)))
                                return false;
                        }
                    }
            }
            catch (Exception e)
            {
                Debug.Print("ERROR: Exception while getting user list: " + e);
                return false;
            }

            return true;
        }

        // Sends log to db
        public bool SendLogs(ArrayList logList)
        {
            var jsonString = Json.BuildNamedArray(LogHeader, logList);
            string url = buildUrlFromSettings();
            var request = WebRequest.Create(url) as HttpWebRequest;
            var requestByteArray = Encoding.UTF8.GetBytes(jsonString);

            try
            {
                // Send the request
                if (request != null)
                {
                    request.Method = "POST";
                    request.ContentType = "application/json; charset=utf-8";
                    request.ContentLength = requestByteArray.Length;

                    var postStream = request.GetRequestStream();

                    postStream.Write(requestByteArray, 0, requestByteArray.Length);
                    postStream.Close();

                    // Grab the response
                    using (var response = request.GetResponse() as HttpWebResponse)
                    {
                        var responseValue = string.Empty;

                        if (response != null)
                        {
                            // Error
                            if (response.StatusCode != HttpStatusCode.OK)
                            {
                                Debug.Print("ERROR: request failed. Received HTTP " + response.StatusCode);
                                return false;
                            }

                            // Grab the response
                            using (var responseStream = response.GetResponseStream())
                            {
                                if (responseStream != null)
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
            }
            catch (Exception e)
            {
                Debug.Print("ERROR: Exception while sending logs: " + e);
                return false;
            }

            return true;
        }

        private string buildUrlFromSettings()
        {
            string ServerIp = SettingsHelper.Get(SettingsHelper.ServerIp);
            string ServerPort = SettingsHelper.Get(SettingsHelper.ServerPort);
            string LockId = SettingsHelper.Get(SettingsHelper.LockId);
            return "http://" + ServerIp + ":" + ServerPort + "/SmartLockRESTService/data/?id=" + LockId;
        }
    }
}