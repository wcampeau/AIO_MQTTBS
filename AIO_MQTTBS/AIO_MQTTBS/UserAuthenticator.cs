using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt.Managers;
using System.IO.IsolatedStorage;
using System.Xml;

namespace AIO_MQTTBS
{
    /// <summary>
    /// User management plugin for GnatMQ broker.
    /// </summary>
    /// <todo>
    /// 1. Securely store user credentials (Win32 credential manager API?)
    /// 2. Provide access to credential store for processes other than service (WCF communication channel)
    /// 3. Provide credential management mechanism (powershell commandlet talking to WCF channel to allow for local or remote WS-Management administration)
    /// 4. Provide local GUI tool
    /// </todo>
    class MQTTUserAuthenticator
    {
        Dictionary<string, string> credentials = new Dictionary<string, string>();

        public MQTTUserAuthenticator ()
        {
            credentials["factory"] = "MM_AIO_FactoryAuthorizationPassword";
            credentials["admin"] = "MM_AIO_AP";
            credentials["user"] = "MM_AIO_user";
        }

        public bool MqttUserAuthenticate(string username, string password)
        {
            return ((credentials.Keys.Contains(username)) && (credentials[username] == password));
        }
    }
}
