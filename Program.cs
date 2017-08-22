using System;
using PushSharp.Apple;
using Newtonsoft.Json.Linq;


namespace pushSharpTest
{
    class Program
    {
        static void Main(string[] args)
        {
            SendiOS();
            Console.WriteLine("Hello World!");
        }

        static void SendiOS() {
            // Configuration (NOTE: .pfx can also be used here)
            var config = new ApnsConfiguration (ApnsConfiguration.ApnsServerEnvironment.Sandbox, 
                "Certificados.p12", "!Ulises31@2011");

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
            MY_DEVICE_TOKENS[0] = "E3C13E2E406307D0FF74494949AF13AD1DC896F751ABCEE0EC72152C19727689";

            foreach (var deviceToken in MY_DEVICE_TOKENS) {
                // Queue a notification to send
                apnsBroker.QueueNotification (new ApnsNotification {
                    DeviceToken = deviceToken,
                    Payload = JObject.Parse ("{\"aps\":{\"badge\":7}}")
                });
            }
            
            // Stop the broker, wait for it to finish   
            // This isn't done after every message, but after you're
            // done with the broker
            apnsBroker.Stop ();            
        }
    }
}
