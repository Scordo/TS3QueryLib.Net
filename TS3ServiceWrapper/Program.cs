using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;

namespace TS3ServiceWrapper
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            if (Environment.UserInteractive)
            {
                if (args != null && args.Length > 0 && (args[0].Equals("/i", StringComparison.InvariantCultureIgnoreCase) || args[0].Equals("/install", StringComparison.InvariantCultureIgnoreCase) || args[0].Equals("-i", StringComparison.InvariantCultureIgnoreCase) || args[0].Equals("-install", StringComparison.InvariantCultureIgnoreCase)))
                {
                    InstallService(args);
                    return;
                }

                if (args != null && args.Length > 0 && (args[0].Equals("/u", StringComparison.InvariantCultureIgnoreCase) || args[0].Equals("/uninstall", StringComparison.InvariantCultureIgnoreCase) || args[0].Equals("-u", StringComparison.InvariantCultureIgnoreCase) || args[0].Equals("-uninstall", StringComparison.InvariantCultureIgnoreCase)))
                {
                    UnInstallService(args);
                    return;
                }

                RunInConsoleMode();
            }
            else
            {
                RunAsService();
            }
        }

        private static void RunAsService()
        {
            int startUpIdleTime;
            int.TryParse(ConfigurationManager.AppSettings["StartupIdle"], out startUpIdleTime);
            Thread.Sleep(startUpIdleTime);
            ServiceBase.Run(new[] { new MainService() });
        }

        private static void RunInConsoleMode()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Running in console mode.");

            HostEngine objWorkflowEngine = new HostEngine();
            objWorkflowEngine.Start();
            Console.WriteLine("Press Enter to stop the service...");
            Console.ReadLine();
            objWorkflowEngine.Stop();
        }

        private static void InstallService(string[] args)
        {
            string strServiceName = GetServiceName(args);

            List<string> objParameters = new List<string> { Assembly.GetExecutingAssembly().Location, string.Format("/LogFile={0}", Assembly.GetExecutingAssembly().Location) };

            if (strServiceName != null && strServiceName.Trim().Length > 0)
                objParameters.Add(string.Format("--ServiceName={0}", strServiceName));

            try
            {
                ManagedInstallerClass.InstallHelper(objParameters.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured during install:");
                Console.WriteLine(ex.ToString());
            }
        }

        private static void UnInstallService(string[] args)
        {
            string strServiceName = GetServiceName(args);

            List<string> parameters = new List<string> { Assembly.GetExecutingAssembly().Location, "/u", string.Format("/LogFile={0}", Assembly.GetExecutingAssembly().Location) };

            if (strServiceName != null && strServiceName.Trim().Length > 0)
                parameters.Add(string.Format("--ServiceName={0}", strServiceName));

            try
            {
                ManagedInstallerClass.InstallHelper(parameters.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured during uninstall:");
                Console.WriteLine(ex.ToString());
            }
        }

        private static string GetServiceName(string[] args)
        {
            if (args.Length > 1 && args[1].StartsWith("/sn=", StringComparison.InvariantCultureIgnoreCase))
            {
                string strServiceName = args[1].Substring(4).Trim().Trim('"');
                return strServiceName.Length == 0 ? null : strServiceName;
            }

            if (args.Length > 1 && args[1].StartsWith("/servicename=", StringComparison.InvariantCultureIgnoreCase))
            {
                string strServiceName = args[1].Substring(13).Trim().Trim('"');
                return strServiceName.Length == 0 ? null : strServiceName;
            }

            return null;
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // handle uncaught exceptions here
        }
    }
}
