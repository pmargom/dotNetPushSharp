using System;
using PushSharp;
using PushSharp.Apple;
using Newtonsoft.Json.Linq;


namespace pushSharpTest
{
    class Program
    {
        static void Main(string[] args)
        {
            SendiOS();
        }

        static void SendiOS() {
            // Configuration (NOTE: .pfx can also be used here)
            var config = new ApnsConfiguration (ApnsConfiguration.ApnsServerEnvironment.Sandbox, 
                "PushSharp-Sandbox.p12", "!Ulises31@2011");

            // Create a new broker
            var apnsBroker = new ApnsServiceBroker (config);
            // Wire up events
            apnsBroker.OnNotificationFailed += (notification, aggregateEx) => {

                aggregateEx.Handle (ex => {
                
                    // See what kind of exception it was to further diagnose
                    if (ex is ApnsNotificationException) {
                        var notificationException = (ApnsNotificationException)ex;
                        
                        // Deal with the failed notification
                        var apnsNotification = notificationException.Notification;
                        var statusCode = notificationException.ErrorStatusCode;

                        Console.WriteLine ($"Apple Notification Failed: ID={apnsNotification.Identifier}, Code={statusCode}");
                
                    } else {
                        // Inner exception might hold more useful information like an ApnsConnectionException			
                        Console.WriteLine ($"Apple Notification Failed for some unknown reason : {ex.InnerException}");
                    }

                    // Mark it as handled
                    return true;
                });
            };

            apnsBroker.OnNotificationSucceeded += (notification) => {
                Console.WriteLine ("Apple Notification Sent!");
            };

            // Start the broker
            apnsBroker.Start ();

            var MY_DEVICE_TOKENS = new string[1];
            MY_DEVICE_TOKENS[0] = "8EB31300771663950212827E212F7B7FB2B34CA26A2D4F3927AC37E5F7E5DB15";
            // MY_DEVICE_TOKENS[1] = "55EE2DF878CAB0D9A0C3B679C9722CC6E600B86AEDF71CDD853FB4CC5AEC9718";

            foreach (var deviceToken in MY_DEVICE_TOKENS) {
                // Queue a notification to send
                apnsBroker.QueueNotification (new ApnsNotification {
                    DeviceToken = deviceToken,
                    Payload = JObject.Parse ("{\"aps\":{\"badge\":90,\"alert\":\"Jerónimo!!\", \"sound\":\"default\"}}")
                });
            }
            
            // Stop the broker, wait for it to finish   
            // This isn't done after every message, but after you're
            // done with the broker
            apnsBroker.Stop ();            
        }
    }
}
