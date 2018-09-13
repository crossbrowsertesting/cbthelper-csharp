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
    public class Snapshot
    { 
        //Represents a snapshot for selenium tests
        
            //Hash for the image. Returned by the API when taking a screenshot
        string hash;
            //Returned in Automated Test Object.  Corresponds to a Selenium test.
        string testId;
            //CrossBrowserTesting Selenium username login information
        string username;
            //CrossBrowserTesting Selenium authkey login information
        string authkey;

        //link to selenium api for CrossBrowserTesting
        public string BaseURL = "https://crossbrowsertesting.com/api/v3/selenium";

        public Snapshot(string hash, AutomatedTest test)
        {
            this.hash = hash;
            this.testId = test.testId;
            this.username = Example.username;
            this.authkey = Example.authkey;
            getInfo();
        }

        public string getInfo()
        {
            //Uses api to get updated information for this snapshot. Returns
            //a string representation of the information in json format


            // encode the data to be written
            ASCIIEncoding encoding = new ASCIIEncoding();
            // create the request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + "/" + testId + "/snapshots/" + hash);
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

            //Download the current snapshot to the given location

            WebClient client = new WebClient();
            string downloadPath = getDownloadUrl(getInfo());
            client.DownloadFile(new Uri(getDownloadUrl(getInfo())), location);
        }
        public string getDownloadUrl(string json)
        {
            //Parses the JSON to get the download url for the snapshot image

            JObject snapJObject = JObject.Parse(json);
            
            {

                foreach(JProperty p in snapJObject.Properties())
                {

                    if(p.Value.Type == JTokenType.String)
                    {
                        string name = p.Name;
                        string value = (string)p.Value;

                        if (name == "image")
                            return value;

                    }
                        
                }



            }




            return "";

        }
        public string setDescription(string description)
        {
            //Sets the description for this snapshot


            // encode the data to be written
            ASCIIEncoding encoding = new ASCIIEncoding();
            string data = "description=" + description;
            byte[] putData = encoding.GetBytes(data);
            // create the request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + "/" + testId + "/snapshots/" + hash);
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

    }
}