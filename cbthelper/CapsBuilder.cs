using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Remote;

namespace cbthelper
{
    public class CapsBuilder
    {
        //Builder for generating selenium capabilities

        OpenQA.Selenium.RemoteSessionSettings desiredCapabilities;

        //the following strings are used to change capabilities of the test
        public string platform;
        public string browse;
        public string width;
        public string height;
        public string name;
        public string version;

        public string username;
        public string authkey;
        public string video;
        public string network;


        //List of available capabilities from the api that are for selenium
        List<Capability> capabilities;

        //Used to build out the capabilities list
        browser tempBrowser;
        //Used to build out the capabilities list
        resolution tempResolution;

        //Link to the Selenium API
        public string BaseURL = "https://crossbrowsertesting.com/api/v3/selenium";

        public CapsBuilder()
        {
            this.username = Globals.username;
            this.authkey = Globals.authkey;
            capabilities = new List<Capability>();
            desiredCapabilities = new OpenQA.Selenium.RemoteSessionSettings();
            parseBrowsers(getBrowsers());

            platform = "";
            browse = "";
            width = "";
            height = "";
            name = "";
            version = "";
            video = "";
            network = "";

        }

        public CapsBuilder withPlatform(string platform)
        {
            /* Sets the platform (OS) the user wants to use.  The string will be compared against the name
             * and api_name properties returned from the api
             */

            this.platform = platform;
            return this;
        }
        public CapsBuilder withBrowser(string browser)
        {
            /* Sets the browser the user wants to use.  The string will be compared against the name and api_name
             * properties returned from the api
             */

            this.browse = browser;
            return this;
        }
        public CapsBuilder withResolution(string width, string height)
        {

            //Sets the screen size for the test

            this.width = width;
            this.height = height;
            return this;
        }
        public CapsBuilder withName(string name)
        {
            //Sets the name that will appear in the web app

            this.name = name;
            return this;
        }
        public CapsBuilder withBuild(string build)
        {
            //Sets the build name in the web app

            this.version = build;
            return this;
        }

        public CapsBuilder withRecordVideo(string record)
        {
            //Sets the record video option for the test in the app

            this.video = record;
            return this;
        }

        public CapsBuilder withRecordNetwork(string record)
        {
            //Sets the record network option for the test in the app

            this.network = record;
            return this;

        }
        public string getBrowsers()
        {
            //Gets a full list of the available browsers and resolutions available

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + "/browsers");
            request.Method = "GET";

            request.Credentials = new NetworkCredential(username, authkey);
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "HttpWebRequest";

            var response = (HttpWebResponse)request.GetResponse();
            // store the response
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            response.Close();
            return responseString;

        }
        public class browser
        {
            public string name;
            public string apiName;
            public string browserName;
            public browser() { }

        }
        public class resolution
        {
            public string width;
            public string height;
            public string name;
            public resolution() { }

        }

        public class Capability
        {
            //Mobile specific capabilities
            public string deviceName;
            public string platformVersion;
            public string platformName;
            public string deviceOrientation;

            //Mobile and Desktop capabilities
            public List<string> browserName = new List<string>();
            public List<browser> browsers = new List<browser>();

            //Desktop specific capabilities
            public string version;
            public string platform;
            public List<resolution> resolutions = new List<resolution>();


            public string apiName;
            public string name;
            public bool mobile;

            public Capability()
            {

                mobile = false;
            }

        }

        public void parseBrowsers(string json)
        {

            //Separates the Json returned from the api into a list of possible
            //Capabilities to be used in the Caps Builder


            JArray browsers = JArray.Parse(getBrowsers());

            foreach (JObject browserChildren in browsers.Children<JObject>())
            {
                foreach (JProperty p in browserChildren.Properties())
                {

                    {
                        if (p.Value.Type == JTokenType.String)
                        {
                            string name = p.Name;

                            string value = (string)p.Value;
                            if (name == "api_name")
                            {

                                Capability newApi = new Capability();
                                newApi.apiName = value;
                                capabilities.Add(newApi);

                            }
                            if (name == "name")
                            {
                                capabilities[capabilities.Count - 1].name = value;


                            }
                        }
                        if (p.Value.Type == JTokenType.Object)
                        {
                            if (p.Name == "caps")
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

        public void parseArray(JProperty jProperty)
        {
            //Parses an array to separate values in the json and convert to objects


            JArray jArray = (JArray)jProperty.Value;

            foreach (JObject jOBrowsers in jArray.Children<JObject>())
            {
                foreach (JProperty p in jOBrowsers.Properties())
                {

                    {
                        if (p.Value.Type == JTokenType.Integer)
                        {
                            string name = p.Name;
                            // if (p.Name != "caps")

                            string value = (string)p.Value;

                            if (jProperty.Name == "resolutions")
                            {
                                if (name == "width")
                                {
                                    tempResolution = new resolution();
                                    tempResolution.width = value;
                                }
                                if (name == "height")
                                    tempResolution.height = value;
                            }

                        }

                        if (p.Value.Type == JTokenType.String)
                        {
                            string name = p.Name;

                            {
                                string value = (string)p.Value;

                                if (jProperty.Name == "browsers")
                                {
                                    if (name == "name")
                                    {
                                        tempBrowser = new browser();
                                        tempBrowser.name = value;

                                    }
                                    if (name == "api_name")
                                    {
                                        tempBrowser.apiName = value;

                                    }



                                }
                                if (jProperty.Name == "resolutions")
                                {

                                    if (name == "name")
                                    {
                                        tempResolution.name = value;
                                        capabilities[capabilities.Count - 1].resolutions.Add(tempResolution);
                                    }

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
                                parseArray(p);
                            if (p.Name == "resolutions")
                                parseArray(p);
                        }
                    }

                }



            }




        }

        public void parseObject(JProperty jProperty)
        {
            //Parses a Json Object for conversion into a .Net object

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

                                if (name == "platform" && jProperty.Name == "caps")
                                {
                                    platform = value;
                                    capabilities[capabilities.Count - 1].platform = value;
                                }
                                if (name == "platformName" && jProperty.Name == "caps")
                                {

                                    capabilities[capabilities.Count - 1].platformName = value;
                                }
                                if (name == "deviceName" && jProperty.Name == "caps")
                                {
                                    capabilities[capabilities.Count - 1].deviceName = value;

                                }
                                if (name == "platformVersion" && jProperty.Name == "platformVersion")
                                {
                                    capabilities[capabilities.Count - 1].platformVersion = value;

                                }
                                if (name == "browserName" && jProperty.Name == "caps")
                                {

                                    tempBrowser.browserName = value;
                                    capabilities[capabilities.Count - 1].browserName.Add(value);
                                    capabilities[capabilities.Count - 1].browsers.Add(tempBrowser);


                                }
                                if (name == "version" && jProperty.Name == "caps")
                                {

                                    version = value;


                                }
                                if (name == "screenResolution" && jProperty.Name == "caps")
                                {



                                }
                                if (name == "type" && jProperty.Name == "browsers")
                                {


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

        public browser bestBrowserNoPlatform(List<browser> options, string target)
        {
            //This finds and returns a perfect match when searching for a browser by api name and name


            browser bestOption = options.ElementAt(0);

            foreach (browser browserOption in options)
            {

                if (target.ToLower().Equals(browserOption.apiName.ToLower()) || target.ToLower().Equals(browserOption.name.ToLower()))
                    return browserOption;


            }








            return bestOption;


        }
        public Capability bestOption(string target)
        {
            //This finds and returns a perfect match for a Capability when searching by api name or name
            Capability bestOption = capabilities.ElementAt(0);
            foreach (Capability capability in capabilities)
            {


                if (target.ToLower().Equals(capability.apiName.ToLower()) || target.ToLower().Equals(capability.name.ToLower()))
                    return capability;


            }
            return bestOption;



        }

        public OpenQA.Selenium.RemoteSessionSettings build()
        {
            //Calls the function that decides which capability to use

            return _choose();
        }
        public OpenQA.Selenium.RemoteSessionSettings _choose()
        {
            //Searches through the capabilities and browsers list to find the best match according to user input 

            Capability caps = new Capability();
            browser browserChosen = new browser();
            if (platform != "")
                caps = bestOption(platform);

            if (browse != "")
                browserChosen = bestBrowserNoPlatform(caps.browsers, browse);




            desiredCapabilities = new OpenQA.Selenium.RemoteSessionSettings();

            if (browse != "")
                desiredCapabilities.AddMetadataSetting("browserName", browserChosen.browserName);
            if (platform != "")
                desiredCapabilities.AddMetadataSetting("platform", caps.platform);
            if (width != "" && height != "")
                desiredCapabilities.AddMetadataSetting("screen_resolution", width + "x" + height);
            if (name != "")
                desiredCapabilities.AddMetadataSetting("name", name);
            if (version != "")
                desiredCapabilities.AddMetadataSetting("build", version);
            desiredCapabilities.AddMetadataSetting("username", username);
            desiredCapabilities.AddMetadataSetting("password", authkey);


            return desiredCapabilities;


        }
    }


}