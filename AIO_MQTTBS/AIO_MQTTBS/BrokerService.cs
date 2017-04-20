using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Fclp;
using uPLibrary.Networking.M2Mqtt;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace AIO_MQTTBS
{
    public partial class BrokerService : ServiceBase
    {

        public BrokerService()
        {
            InitializeComponent();
        }

        public class ApplicationArguments
        {
            public int Port { get; set; }
        }

        private MqttBroker _broker = null;
        private MQTTUserAuthenticator _userAuth = new MQTTUserAuthenticator();

        public void DebugStart( string[] args)
        {
            OnStart(args);
        }

        protected override void OnStart(string[] args)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string dir = Path.GetDirectoryName(asm.Location);
            string PFXLocation = Path.Combine(dir, "GnatMQ.pfx");
            if (File.Exists(PFXLocation) == false)
            {
                throw new uPLibrary.Networking.M2Mqtt.Exceptions.MqttConnectionException("GnatMQ.pfx not found", new FileNotFoundException(dir));
            }
            else
            {
                using (FileStream fs = File.OpenRead(PFXLocation))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        fs.CopyTo(ms);

                        X509Certificate2 serverCert = new X509Certificate2(ms.ToArray(), "password");

                        _broker = new MqttBroker(serverCert, MqttSslProtocols.SSLv3);
                        _broker.UserAuth = _userAuth.MqttUserAuthenticate;
                        _broker.Start();
                    }
                }
            }
        }

        protected override void OnStop()
        {
            if( _broker != null)
            {
                _broker.Stop();
            }
        }
    }
}
