using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Reflection;
using System.IO;


namespace MQTTClientConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
            if (p.Connect())
            {
                p.Subscribe();
                p.Publish();
                p.Unsubscribe();
                p.Disconnect();
            }

            Console.WriteLine("Press <enter> to exit");
            Console.ReadLine();
        }


        // No SSL connection
        //        MqttClient client = new MqttClient("10.1.10.65");

        // SSL connection
        MqttClient client =
            new MqttClient(
             "10.1.10.94",
             8883,                  // standard secure MQTT port
             true,                  // connection is secured
             MqttSslProtocols.SSLv3,
             validationCallback,
             certificateSelectionCallback);

        private static X509Certificate certificateSelectionCallback(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            X509Certificate2 myCert = null;

            // retrieve client certificate from disk
            Assembly asm = Assembly.GetExecutingAssembly();
            string dir = Path.GetDirectoryName(asm.Location);
            string CERLocation = Path.Combine(dir, "GnatMQ.cer");
            if (File.Exists(CERLocation) == false)
            {
                throw new uPLibrary.Networking.M2Mqtt.Exceptions.MqttConnectionException("GnatMQ.cer not found", new FileNotFoundException(dir));
            }
            else
            {
                using (FileStream fs = File.OpenRead(CERLocation))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        fs.CopyTo(ms);

                        // REVIEW: XML file to provide certificate password? CredAPI?
                        myCert = new X509Certificate2(ms.ToArray(), "password");
                    }
                }                
            }
            return myCert;
        }

        private static bool validationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // REVIEW: more validation could be done here
            DateTime from = DateTime.Parse(certificate.GetEffectiveDateString());
            DateTime to =   DateTime.Parse(certificate.GetExpirationDateString());
            DateTime now =  DateTime.Now;
            return ((from < now) && (to > now));
        }

        bool Connect()
        {
            bool succeeded = false;
            try
            {
                client.Connect("remote client", "user", "MM_AIO_user");
                client.MqttMsgPublishReceived += (o, e) => { Console.WriteLine("{0} :: {1}", e.Topic, System.Text.UTF8Encoding.UTF8.GetString(e.Message)); };
                client.MqttMsgSubscribed += (o, e) => { Console.WriteLine("subscibed {0}", e.MessageId); };
                client.MqttMsgUnsubscribed += (o, e) => { Console.WriteLine("unsibscribed {0}", e.MessageId); };
                succeeded = true;
            }
            catch (uPLibrary.Networking.M2Mqtt.Exceptions.MqttConnectionException e)
            {
                Console.WriteLine("{0} : {1}", e.Message, e.InnerException.Message);
            }

            return succeeded;
        }

        void Subscribe()
        {
            client.Subscribe(new string[] { "foo/bar" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE } );
        }


        void Publish()
        {
            for (int n = 0; n < 100; n++)
            {
                client.Publish("foo/bar", Encoding.UTF8.GetBytes(DateTime.Now.ToLongTimeString()), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                System.Threading.Thread.Sleep(100);
            }
        }

        void Unsubscribe()
        {
            client.Unsubscribe(new string[] { "foo/bar" });
        }

        void Disconnect()
        {
            client.Disconnect();
        }

    }
}
