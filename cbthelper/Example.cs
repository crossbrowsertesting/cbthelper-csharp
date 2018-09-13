using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Remote;
using System.Net;

namespace cbthelper
{
    public class Example
    {
        //Used to pass login information to CrossBrowserTesting Selenium Hub
        //Globals to be accessed from the other files
        public static string username = "Your User Name";
        public static string authkey = "Your auth key";
        static void Main(string[] args)
        {
     
            //Options on what capabilities to include 
            CapsBuilder caps = new CapsBuilder().withPlatform("Windows 10").withBrowser("Google Chrome 65").withResolution("1024", "768").withName("cbthelper test").withBuild("0.0.1");

            //Set the Capabilities object
            DesiredCapabilities desiredCaps = caps.build();


            //Get a driver to the Selenium hub
            RemoteWebDriver driver = new RemoteWebDriver(new Uri("http://hub.crossbrowsertesting.com:80/wd/hub"), desiredCaps, TimeSpan.FromSeconds(180));


            
            
            //Make an automated test passing in the session id from the Selenium Driver
            AutomatedTest myTest = new AutomatedTest(driver.SessionId.ToString());

            //Save the hash for the Video as you start recording
            string videoHash = myTest.startRecordingVideo();

            //Make a video object with the hash and test information
            Video testVideo = new Video(videoHash, myTest);

            //Navigate to google
            driver.Navigate().GoToUrl("http://www.google.com");

            //Take a snapsot with the api
            string googleHash = myTest.takeSnapshot();

            //Make a snapshot object with the hash and test information
            Snapshot googleSnap = new Snapshot(googleHash, myTest);

            //Set a description for the snapshot
            googleSnap.setDescription("google.com");

            //Navigate to Cross Browser Testing
            driver.Navigate().GoToUrl("http://crossbrowsertesting.com");

            //Use the api to take a snapshot of the webpage
            string cbtHash = myTest.takeSnapshot("cbt.com");

            //Save the snapshot.  Make sure the snapshot has time to finish being processed before calling this
            googleSnap.saveLocally("Path and filename to save to");


            //Use the api to stop recording video
            testVideo.stopRecording();
            
            //Use the api to set the score test
            myTest.setScore("pass");

            //Use the api to set the description for the test
            myTest.setDescription("Cbt helper test");

            //Stop the automated test
            myTest.stop();

            //CLose the driver ending the Selenium test.
            driver.Quit();


            //Save the video to a file.  Make sure that the video has had time to finish processing
            //before calling this or you will not get the video
            testVideo.saveLocally("Path and filename to save to");

            //Make a builder object to get test history
            TestHistoryBuilder options = new TestHistoryBuilder();

            //Set the name to get test history results with
            options.withName("cbthelper test");

            //Set the limit for number of tests to return
            options.withLimit("5");

            //return the json as a string for the test history api call
            options.getTestHistory();


        }
    }
}
