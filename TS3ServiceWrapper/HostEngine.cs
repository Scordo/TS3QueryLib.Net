using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
namespace TS3ServiceWrapper
{
    public class HostEngine
    {
        private Process StartedProcess { get; set; }
        private BackgroundWorker Worker { get; set; }
        private string WorkingDirectory { get; set; }
        private string StartupFile { get; set; }
        private string StartupParameters { get; set; }
        private bool HideProcess { get; set; }

        public void Start()
        {
            StartupFile = ConfigurationManager.AppSettings["StartupFile"];

            if (StartupFile == null)
                throw new ConfigurationErrorsException("Please provide a StartupFile in config.");

            StartupParameters = ConfigurationManager.AppSettings["StartupParameters"];
            WorkingDirectory = ConfigurationManager.AppSettings["WorkingDirectory"];
            HideProcess = string.Compare("true", ConfigurationManager.AppSettings["HideProcess"], true) == 0;

            Worker = new BackgroundWorker { WorkerSupportsCancellation = true };
            Worker.DoWork += Worker_DoWork;
            Worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            StartedProcess = new Process();
            StartedProcess.StartInfo.FileName = StartupFile;
            StartedProcess.StartInfo.Arguments = StartupParameters;
            StartedProcess.StartInfo.WorkingDirectory = WorkingDirectory;
            StartedProcess.StartInfo.UseShellExecute = true;

            if (HideProcess)
            {
                StartedProcess.StartInfo.CreateNoWindow = true;
                StartedProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }

            StartedProcess.Start();
            StartedProcess.WaitForExit();

            if (!((BackgroundWorker)sender).CancellationPending)
                Start();
        }

        public void Stop()
        {
            Worker.CancelAsync();

            if (StartedProcess != null)
                StartedProcess.Kill();
        }
    }
}