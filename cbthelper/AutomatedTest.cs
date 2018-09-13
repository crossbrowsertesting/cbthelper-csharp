using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace cbthelper
{
    public class AutomatedTest
    {
        //The Selenium session ID, usually from webdriver
        public string testId;
       //The username automation login information for CrossBrowserTesting
        string username;
        //The authkey automation login information for CrossBrowserTesting
        string authkey; 

        public string BaseURL = "https://crossbrowsertesting.com/api/v3/selenium";
        public AutomatedTest(string testId)
        {
            this.testId = testId;
            this.username = Example.username;
            this.authkey = Example.authkey;
        }


        public void setScore(string score)
        {

            /*
            * Sets the score for a test in the CrossBrowserTesting application.
            * Score should be pass, fail, or unset
            */

            string url = BaseURL + "/" + testId;
            // encode the data to be written
            ASCIIEncoding encoding = new ASCIIEncoding();
            string data = "action=set_score&score=" + score;
            byte[] putdata = encoding.GetBytes(data);
            // Create the request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "PUT";
            request.Credentials = new NetworkCredential(username, authkey);
            request.ContentLength = putdata.Length;
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "HttpWebRequest";
            // Write data to stream
            Stream newStream = request.GetRequestStream();
            newStream.Write(putdata, 0, putdata.Length);
            newStream.Close();
            WebResponse response = request.GetResponse();

            response.Close();
        }
        public void setDescription(string description)
        {
            /*
             * Sets the description for the test in the CrossBrowserTesting application.
             *
             */


            string url = BaseURL + "/" + testId;
            // encode the data to be written
            ASCIIEncoding encoding = new ASCIIEncoding();
            string data = "action=set_description&description=" + description;
            byte[] putdata = encoding.GetBytes(data);
            // Create the request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "PUT";
            request.Credentials = new NetworkCredential(username, authkey);
            request.ContentLength = putdata.Length;
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "HttpWebRequest";
            // Write data to stream
            Stream newStream = request.GetRequestStream();
            newStream.Write(putdata, 0, putdata.Length);
            newStream.Close();
            WebResponse response = request.GetResponse();
            response.Close();
        }
        public string stop(string score = "")
        {

            /*
             * Stops the test in the application.
             * Return the response from the server
            */

            if (score != "")
                setScore(score);
            // encode the data to be written
            ASCIIEncoding encoding = new ASCIIEncoding();
            // create the request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + "/" + testId);
            request.Method = "DELETE";
            request.Credentials = new NetworkCredential(username, authkey);
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "HttpWebRequest";

            WebResponse response = request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            response.Close();
            return responseString;
        }
        public string takeSnapshot(string description = "")
        {
            /* 
             * 
             * Takes a snapshot in a selenium test of the browser window
             *returns the screenshot hash to be used in the setDescription method.
             * 
             */



            //create the POST request object pointed at the snapshot endpoint

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + "/" + testId + "/snapshots");

            request.Method = "POST";
            request.Credentials = new NetworkCredential(username, authkey);
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "HttpWebRequest";
            // Execute the request
            var response = (HttpWebResponse)request.GetResponse();
            // store the response
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            // parse out the snapshot Hash value 
            var myregex = new Regex("(?<=\"hash\": \")((\\w|\\d)*)");
            var snapshotHash = myregex.Match(responseString).Value;

            Snapshot snap = new Snapshot(snapshotHash, this);
            if (description != "")
                snap.setDescription(description);
            response.Close();
            return snapshotHash;
        }


        public string startRecordingVideo(string description = "")
        {
            /* 
            * Starts recording video of the selenium session
            * returns the video hash to be used in the setDescription method.
            */

            // create the POST request object pointed at the snapshot endpoint
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + "/" + testId + "/videos");

            request.Method = "POST";
            request.Credentials = new NetworkCredential(username, authkey);
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "HttpWebRequest";
            // Execute the request
            var response = (HttpWebResponse)request.GetResponse();
            // store the response
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            // parse out the snapshot Hash value 
            var myregex = new Regex("(?<=\"hash\": \")((\\w|\\d)*)");
            var videoHash = myregex.Match(responseString).Value;

            Video video = new Video(videoHash, this);
            if (description != "")
                video.setDescription(description);
            response.Close();
            return videoHash;
        }
    }


}