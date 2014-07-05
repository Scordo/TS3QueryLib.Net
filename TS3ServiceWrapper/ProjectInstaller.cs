using System;
using System.ComponentModel;
using System.Configuration.Install;

namespace TS3ServiceWrapper
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();

            const string paramName = "--ServiceName=";
            int index = Environment.CommandLine.IndexOf(paramName, StringComparison.InvariantCultureIgnoreCase);

            if (index == -1)
                return;

            ServiceInstaller.ServiceName = Environment.CommandLine.Substring(index + paramName.Length).Trim().Trim('"');
            ServiceInstaller.DisplayName = ServiceInstaller.ServiceName;
            Console.WriteLine("Installing as '" + ServiceInstaller.ServiceName + "'");
        }
    }
}