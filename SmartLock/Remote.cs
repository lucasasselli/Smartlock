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
        public static Result Get(string url)
        {
            DebugOnly.Print("Performing GET...");
            DebugOnly.Print("\t\tURL: " + url);

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
                            DebugOnly.Print("ERROR: request failed. Received HTTP " + response.StatusCode);
                            return new Result(false, null);
                        }

                        var responseStream = response.GetResponseStream();
                        var responseReader = new StreamReader(responseStream);
                        responseString = responseReader.ReadToEnd();
                        responseStream.Close();
                        responseReader.Close();

                        DebugOnly.Print("\t\tResponse: " + responseString);
                        return new Result(true, responseString);
                        
                    }
                }
            }
            catch (Exception e)
            {
                DebugOnly.Print("ERROR: Exception while performing GET: " + e);
            }

            return new Result(false, null);
        }


        public static Result Post(string url, string body)
        {
            var request = WebRequest.Create(url) as HttpWebRequest;
            var requestByteArray = Encoding.UTF8.GetBytes(body);

            DebugOnly.Print("Performing POST...");
            DebugOnly.Print("\t\tURL: " + url);
            DebugOnly.Print("\t\tBody: " + body);

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
                        var responseString = string.Empty;

                        // Error
                        if (response == null || response.StatusCode != HttpStatusCode.OK)
                        {
                            DebugOnly.Print("ERROR: request failed. Received HTTP " + response.StatusCode);
                            return new Result(false, null);
                        }

                        // Grab the response
                        using (var responseStream = response.GetResponseStream())
                        {
                            if (responseStream != null)
                            {
                                using (var reader = new StreamReader(responseStream))
                                {
                                    responseString = reader.ReadToEnd();
                                }

                                DebugOnly.Print("\t\tResponse: " + responseString);
                                return new Result(true, responseString);
                            }
                        }                     
                    }
                }
            }
            catch (Exception e)
            {
                DebugOnly.Print("ERROR: Exception while performing POST: " + e);
                return new Result(false, null);
            }

            return new Result(false, null);
        }

        public class Result
        {
            public bool Success { private set; get; }
            public string Content { private set; get; }

            internal Result(bool success, string content)
            {
                Success = success;
                Content = content;
            }
        }
    }
}
