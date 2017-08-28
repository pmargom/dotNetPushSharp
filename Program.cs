using System;
using System.Collections.Generic;
using PushSharp;
using PushSharp.Core;
using PushSharp.Apple;
using PushSharp.Google;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Text;

namespace pushSharpTest
{
    class Program
    {

        private static string DeviceID = "c8XLbUVxFbA:APA91bETeDNng_CNKzypyFVa0AlSJPjWoOWtB1e0d92vZDiwWYcWWUzr6Mw0XZmDCQI1m_tIx0jjdNmZC_u3YaLeJLSqUKrn0lcwfEmdWr4EC9JEYDYiSZyBkhZ1M5vumlMNDheEQdmv";
        // private static string SENDER_ID = "849171215333";
        private static string applicationID = "AAAA4qLXWQI:APA91bHcdacnEUcB-3F-vR3NpvlkTK7ccxTZWocWY4a3UgngNHd0nFyWkJXWs0i4H5gCPXFS_JGt65Pdz9fTLayeoN9VFxwzJC7svqCrfkMP9Uc6us0WlsbsnnIFbhAnrjFSXDpSBys0";

        private static string firebaseUrl = "https://fcm.googleapis.com/fcm/send";
        
        static void Main(string[] args)
        {
            // SendiOS();
            SendAndroid();
            // send();
        }

        // static void send()
            // {                
            //     var value = "Pruebaaaaa";
            //     WebRequest tRequest;

            //     tRequest = WebRequest.Create(firebaseUrl);
            //     tRequest.Method = "post";
            //     tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
            //     tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));

            //     tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));

            //     // string postData = "{ 'registration_id': [ '" + DeviceID + "' ], 'data': {'message': '" + txtMsg.Text + "'}}";
            //     //string postData = "collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.message=" + value + "&data.time=" + System.DateTime.Now.ToString() + "&registration_id=" + DeviceID + "";
            //     string collaspeKey = Guid.NewGuid().ToString("n");
            //     string postData=string.Format("registration_id={0}&data.payload={1}&collapse_key={2}", DeviceID, "Pickup Message" + DateTime.Now.ToString(), collaspeKey);

            //     Console.WriteLine(postData);
            //     Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            //     tRequest.ContentLength = byteArray.Length;

            //     Stream dataStream = tRequest.GetRequestStream();
            //     dataStream.Write(byteArray, 0, byteArray.Length);
            //     dataStream.Close();

            //     WebResponse tResponse = tRequest.GetResponse();

            //     dataStream = tResponse.GetResponseStream();

            //     StreamReader tReader = new StreamReader(dataStream);

            //     String sResponseFromServer = tReader.ReadToEnd();

            //     Console.WriteLine("Respuesta server -> " + sResponseFromServer);
            //     tReader.Close();
            //     dataStream.Close();
            //     tResponse.Close();
            // }

        static void SendAndroid() {
            // Configuration
            //var config = new GcmConfiguration ("GCM-SENDER-ID", "AUTH-TOKEN", null);
            var config = new GcmConfiguration(applicationID);
            config.GcmUrl = firebaseUrl;

            // Create a new broker
            var gcmBroker = new GcmServiceBroker (config);
                
            // Wire up events
            gcmBroker.OnNotificationFailed += (notification, aggregateEx) => {

                aggregateEx.Handle (ex => {
                
                    // See what kind of exception it was to further diagnose
                    if (ex is GcmNotificationException) {
                        var notificationException = (GcmNotificationException)ex;
                        
                        // Deal with the failed notification
                        var gcmNotification = notificationException.Notification;
                        var description = notificationException.Description;

                        Console.WriteLine ($"GCM Notification Failed: ID={gcmNotification.MessageId}, Desc={description}");
                    } else if (ex is GcmMulticastResultException) {
                        var multicastException = (GcmMulticastResultException)ex;

                        foreach (var succeededNotification in multicastException.Succeeded) {
                            Console.WriteLine ($"GCM Notification Succeeded: ID={succeededNotification.MessageId}");
                        }

                        foreach (var failedKvp in multicastException.Failed) {
                            var n = failedKvp.Key;
                            var e = failedKvp.Value;

                            //Console.WriteLine ($"GCM Notification Failed: ID={n.MessageId}, Desc={e.Description}");
                            Console.WriteLine ($"GCM Notification Failed: ID={n.MessageId}");
                            Console.WriteLine ($"GCM Notification EXCEPTION: {e.Message}");
                        }

                    } else if (ex is DeviceSubscriptionExpiredException) {
                        var expiredException = (DeviceSubscriptionExpiredException)ex;
                        
                        var oldId = expiredException.OldSubscriptionId;
                        var newId = expiredException.NewSubscriptionId;

                        Console.WriteLine ($"Device RegistrationId Expired: {oldId}");

                        //if (!string.IsNullOrWhitespace (newId)) {
                        if (!string.IsNullOrEmpty (newId)) {
                            // If this value isn't null, our subscription changed and we should update our database
                            Console.WriteLine ($"Device RegistrationId Changed To: {newId}");
                        }
                    } else if (ex is RetryAfterException) {
                        var retryException = (RetryAfterException)ex;
                        // If you get rate limited, you should stop sending messages until after the RetryAfterUtc date
                        Console.WriteLine ($"GCM Rate Limited, don't send more until after {retryException.RetryAfterUtc}");
                    } else {
                        Console.WriteLine ("GCM Notification Failed for some unknown reason");
                    }

                    // Mark it as handled
                    return true;
                });
            };

            gcmBroker.OnNotificationSucceeded += (notification) => {
                Console.WriteLine ("GCM Notification Sent!");
            };

            // Start the broker
            gcmBroker.Start ();

            var MY_REGISTRATION_IDS = new string[2];
            // SOBRE ESCRIBO EL PRIMER DEVICEID
            // SIMULADOR
            DeviceID = "eQvAo4vE-yo:APA91bGdtBqKAFJuBD0HP6SwfSq8u38Y51SSfrbVteqAVHG6wXfc7fVHFuU9pDj0igNr7b55mBo1iK68H5VQK5TFipLgQKd9CeJ6JnBhV68doN3oxMJpExt4n_sZGx02J6nI19zf3-Nf";
            MY_REGISTRATION_IDS[0] = DeviceID;
            MY_REGISTRATION_IDS[1] = "e3Gq20HUiso:APA91bEI8hqfVDI3Rq-RXPUk2oM2hYD0CJgMJzX2SS9bJkm37vTDNLCMx9moKLTrXKjCw1SFDjKEW8QzkgLh-sUrZI4Qw5MMxNvEoORH_FGWXgt58g09eQ40deBgzwbbKj-RkZRgfO4z";

            foreach (var deviceId in MY_REGISTRATION_IDS) {
                // Queue a notification to send
                Console.WriteLine($"Enviando notificación a dispositivo: {deviceId}");
                gcmBroker.QueueNotification (new GcmNotification {
                    RegistrationIds = new List<string> { 
                        deviceId
                    },
                    Data = JObject.Parse ("{ \"message\" : \"desde PushSharp\" }")
                });
            }
            
            // Stop the broker, wait for it to finish   
            // This isn't done after every message, but after you're
            // done with the broker
            gcmBroker.Stop ();
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
