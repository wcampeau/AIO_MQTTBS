using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration.Install;
using System.Reflection;
using System.Security.Principal;
using System.Security.Claims;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace AIO_MQTTBS
{
    static class Program
    {
        private static bool IsAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;

            if (System.Environment.UserInteractive)
            {
                string parameter = (args.Count() > 0) ? args[0] : "";
                switch (parameter)
                {
                    case "--install":
                    {
                        if (IsAdmin())
                        {
                            ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                        }
                        else
                        {
                                Console.WriteLine("Please install with elevated privileges.");
                        }
                        break;
                    }
                    case "--uninstall":
                    {
                        if( IsAdmin() )
                        { 
                            ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                        }
                        else
                        {
                                Console.WriteLine("Please uninstall with elevated privileges.");
                        }
                        break;
                    }
                    case "--debug":
                    {
                        BrokerService service = new BrokerService();
                        service.DebugStart(args);

                        WindowsIdentity identity = WindowsIdentity.GetCurrent();
                        Console.WriteLine("Broker running as {0}.", identity.Name);
                        Console.WriteLine("Press <enter> key to exit.");
                        Console.ReadLine();

                        service.Stop();

                        break;
                    }
                    case "--start":
                    {
                        ServiceController service = new ServiceController(ServiceInformation.Name);
                        try
                        {
                            TimeSpan timeout = TimeSpan.FromMilliseconds(2000);

                            service.Start();
                            service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;                        
                    }
                    case "--stop":
                    {
                        ServiceController service = new ServiceController(ServiceInformation.Name);
                        try
                        {
                            TimeSpan timeout = TimeSpan.FromMilliseconds(2000);

                            service.Stop();
                            service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    }
                    default:
                    {
                        Console.WriteLine("AIO_MQTBS [--install] [--uninstall] [--start] [--stop]");
                        break;
                    }
                }
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new BrokerService()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }

        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ToString());
        }

    }
}
