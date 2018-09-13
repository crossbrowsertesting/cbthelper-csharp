using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace cbthelper
{
    public class TestHistoryBuilder
    {
        //Builder to generate options for getting test history

            //Options for changing the configuration for the test history api call
        private string num;
        private string active;
        private string name;
        private string build;
        private string url;
        private string score;
        private string platform;
        private string platformType;
        private string browser;
        private string browserType;
        private string resolution;
        private string startDate;
        private string endDate;


        //Login information for the Cross Browser Testing Selenium connection
        private string username;
        //Login information for the Cross Browser Testing Selenium connection
        private string authkey;
      

        public string BaseURL = "https://crossbrowsertesting.com/api/v3/selenium";

        public TestHistoryBuilder()
        {
            this.username = Example.username;
            this.authkey = Example.authkey;
           
            num = "num=";
            active = "&active=";
            name = "&name=";
            build = "&build=";
            url = "&url=";
            score = "&score=";
            platform = "&os=";
            platformType = "&os_type=";
            browser = "&browser=";
            browserType = "&browse_type=";
            resolution = "&resolution=";
            startDate = "&start_date=";
            endDate = "&end_date=";


        }

        //The following functions that begin with "with" are used to set the configuration for the api call

        public void withLimit(string num)
        {
            this.num = "num=" + num;
        }
        public void withActive(string active)
        {
            this.active = "&active=" + active;
        }
        public void withName(string name)
        {
            this.name = "&name=" + name;
        }
        public void withBuild(string build)
        {
            this.build = "&build=" + build;
        }
        public void withUrl(string url)
        {
            this.url = "&url=" + url;
        }
        public void withScore(string score)
        {
            this.score = "&score=" + score;
        }
        public void withPlatform(string platform)
        {
            this.platform = "&os=" + platform;
        }
        public void withPlatformType(string platformType)
        {
            this.platformType = "&os_type=" + platformType;
        }
        public void withBrowser(string browser)
        {
            this.browser = "&browser=" + browser;
        }
        public void withBrowserType(string browserType)
        {
            this.browserType = "&browser_type=" + browserType;
        }
        public void withResolution(string resolution)
        {
            this.resolution = "&resolution=" + resolution;
        }
        public void withStartDate(string startDate)
        {
            this.startDate = "&start_date=" + startDate;
        }
        public void withEndDate(string endDate)
        {
            this.endDate = "&end_date=" + endDate;
        }
    

        //Used to parse JSON and turn it into a .Net object
        public void parseObject(JProperty jProperty)
        {
           

            JObject jObject = (JObject)jProperty.Value;

          
            {
                foreach (JProperty p in jObject.Properties())
                {

                    {
                        if (p.Value.Type == JTokenType.String)
                        {
                            string name = p.Name;
                          
                            {
                                string value = (string)p.Value;

                                if (name == "selenium_test_id")
                                {
                                    Console.WriteLine(value);
                                }

                                
                            }
                        }
                        if (p.Value.Type == JTokenType.Object)
                        {
                            if (p.Name == "caps")
                                parseObject(p);

                        }
                        if (p.Value.Type == JTokenType.Array)
                        {
                            if (p.Name == "browsers")
                            {

                                parseArray(p);

                            }

                        }
                    }

                }



            }




        }

        //Used to parse Json and turn it into a .Net object

        public void parseArray(JProperty jProperty)
        {
            JArray jArray = (JArray)jProperty.Value;

            foreach (JObject jOBrowsers in jArray.Children<JObject>())
            {
                foreach (JProperty p in jOBrowsers.Properties())
                {

                    {
                        if (p.Value.Type == JTokenType.Integer)
                        {
                            string name = p.Name;
                            string value = (string)p.Value;

                            if (name == "selenium_test_id")
                            {
                                
                            }
                        }
                        if (p.Value.Type == JTokenType.String)
                        {
                            string name = p.Name;
                            {
                                string value = (string)p.Value;

                                if (name == "selenium_test_id")
                                { }
        
                        
                            }
                        }
                        if (p.Value.Type == JTokenType.Object)
                        {
                            parseObject(p);

                        }
                        if (p.Value.Type == JTokenType.Array)
                        {
                            parseArray(p);


                        }
                    }

                }



            }




        }

        //Makes the request with the specified parameters and returns the response as a string in json format
        public string getTestHistory()
        {


            ASCIIEncoding encoding = new ASCIIEncoding();

            string getData = (num + active + name + build + url + score + platform + platformType + browser + browserType + resolution + startDate + endDate);


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + "?" + getData);
            request.Method = "GET";

            request.Credentials = new NetworkCredential(username, authkey);
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "HttpWebRequest";

            var response = (HttpWebResponse)request.GetResponse();
          
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();




            response.Close();

            return responseString;

        }
    }
}