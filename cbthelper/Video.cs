using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace cbthelper
{
    public class Video
    {
        //Hash for the video returned by the api when starting a recording
        string hash;
        //Automated test object that represents currently running test
        string testId;
        //Login information for CrossBrowserTesting Selenium
        string username;
        //Login information for CrossBrowserTesting Selenium
        string authkey;

        public string BaseURL = "https://crossbrowsertesting.com/api/v3/selenium";

        public Video(string hash, AutomatedTest test)
        {
            this.hash = hash;
            this.testId = test.testId;
            this.username = Example.username;
            this.authkey = Example.authkey;
            getInfo();
        }

        public string getInfo()
        {


            //Gets the information for this video from the api
            //returns the response from the api in string format

            // encode the data to be written
            ASCIIEncoding encoding = new ASCIIEncoding();
            // create the request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + "/" + testId + "/videos/" + hash);
            request.Method = "GET";
            request.Credentials = new NetworkCredential(username, authkey);
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "HttpWebRequest";
            WebResponse response = request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();


            response.Close();
            return responseString;


        }

        public void saveLocally(string location)
        {
            //Download the video and save to location given

            //location should have the location and filename for the video and the filename should
            //be in .mp4 format


            WebClient client = new WebClient();
            string downloadPath = getDownloadUrl(getInfo());
            client.DownloadFile(new Uri(getDownloadUrl(getInfo())), location);
        }
        public string getDownloadUrl(string json)
        {
            //Parses the Json respobnse from the api to get the location to download 
            //the video from

            JObject snapJObject = JObject.Parse(json);

           
            {

                foreach (JProperty p in snapJObject.Properties())
                {

                    if (p.Value.Type == JTokenType.String)
                    {
                        string name = p.Name;
                        string value = (string)p.Value;

                        if (name == "video")
                            return value;

                    }

                }



            }




            return "";

        }
        public string setDescription(string description)
        {
            //sets the description for the video

            // encode the data to be written
            ASCIIEncoding encoding = new ASCIIEncoding();
            string data = "description=" + description;
            byte[] putData = encoding.GetBytes(data);
            // create the request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + "/" + testId + "/videos/" + hash);
            request.Method = "PUT";
            request.Credentials = new NetworkCredential(username, authkey);
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "HttpWebRequest";
            // write data to stream
            Stream newStream = request.GetRequestStream();
            newStream.Write(putData, 0, putData.Length);
            newStream.Close();
            WebResponse response = request.GetResponse();


            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            response.Close();
            return responseString;
        }

        public string stopRecording()
        {

            //Sends the command to stop a video recording

            // encode the data to be written
            ASCIIEncoding encoding = new ASCIIEncoding();
            // create the request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + "/" + testId + "/videos/" + hash);
            request.Method = "DELETE";
            request.Credentials = new NetworkCredential(username, authkey);
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "HttpWebRequest";

            WebResponse response = request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            response.Close();
            return responseString;
        }
    }
}
