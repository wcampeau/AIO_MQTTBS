using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace AIO_MQTTBS
{
    [RunInstaller(true)]
    public class MQTTBrokerServiceInstaller : Installer
    {
        public MQTTBrokerServiceInstaller()
        {
            var processInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();

            processInstaller.Account = ServiceAccount.NetworkService;

            serviceInstaller.DisplayName = ServiceInformation.DisplayName;
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            serviceInstaller.ServiceName = ServiceInformation.Name;
            this.Installers.Add(processInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
}