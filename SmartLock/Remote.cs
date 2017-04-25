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
    class Remote
    {
        // Request current timestamp
        public static RemoteResponse Get(string url)
        {
            string responseString = null;
            var request = WebRequest.Create(url) as HttpWebRequest;
            try
            {
                if (request != null)
                {
                    using (var response = request.GetResponse() as HttpWebResponse)
                    {
                        // Error
                        if (response == null || response.StatusCode != HttpStatusCode.OK)
                        {
                            Debug.Print("ERROR: request failed. Received HTTP " + response.StatusCode);
                            return new RemoteResponse(false, null);
                        }

                        var responseStream = response.GetResponseStream();
                        var responseReader = new StreamReader(responseStream);
                        responseString = responseReader.ReadToEnd();
                        responseStream.Close();
                        responseReader.Close();

                        return new RemoteResponse(true, responseString);
                        
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Print("ERROR: Exception while performing GET: " + e);
            }

            return new RemoteResponse(false, null);
        }


        public static RemoteResponse Post(string url, string body)
        {
            var request = WebRequest.Create(url) as HttpWebRequest;
            var requestByteArray = Encoding.UTF8.GetBytes(body);

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

                        // Error
                        if (response == null || response.StatusCode != HttpStatusCode.OK)
                        {
                            Debug.Print("ERROR: request failed. Received HTTP " + response.StatusCode);
                            return new RemoteResponse(false, null);
                        }

                        // Grab the response
                        using (var responseStream = response.GetResponseStream())
                        {
                            if (responseStream != null)
                            {
                                using (var reader = new StreamReader(responseStream))
                                {
                                    responseValue = reader.ReadToEnd();
                                }

                                return new RemoteResponse(true, responseValue);
                            }
                        }                     
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Print("ERROR: Exception while performing POST: " + e);
                return new RemoteResponse(false, null);
            }

            return new RemoteResponse(false, null);
        }
    }

    public class RemoteResponse
    {
        bool success { private set; get; }
        string content { private set; get; }

        internal RemoteResponse(bool success, string content)
        {
            this.success = success;
            this.content = content;
        }
    }

}
