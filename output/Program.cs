using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Topshelf;

namespace HostFileReplace
{
    class FileReplacer:Topshelf.ServiceControl
    {
        private static string fromFile = Path.Combine(Environment.SystemDirectory, "drivers\\etc\\hosts_src");
        private static string toFile = Path.Combine(Environment.SystemDirectory, "drivers\\etc\\hosts");
        private static string addendum = Path.Combine(Environment.SystemDirectory, "drivers\\etc\\hosts_addendum.txt");
      
        private static AutoResetEvent waitHandle = new AutoResetEvent(true);
        private static AutoResetEvent copyHandle = new AutoResetEvent(false);
        private static AutoResetEvent unlockHandle = new AutoResetEvent(false);
        private static DateTime lastUpdate = DateTime.MinValue;
        private static bool threadDone = false;
        private static IDictionary<string, string> entries;

        // Algorithm for file replacer
        // Source file should be locked at all time until it is ready to be read
        // Every few seconds, check if the file has an update
        // if there is, signal locker to release lock 
        // get lock to copy file
        void ReplaceFile()
        {
            
            FileInfo fi = new FileInfo(fromFile);
            if (fi.LastWriteTime > lastUpdate)
            {
                lastUpdate = fi.LastWriteTime;
                unlockHandle.Set();
                copyHandle.WaitOne();
                try
                {
                    File.Copy(fromFile, toFile, true);
                    waitHandle.Set(); // can lock file
                }
                catch (Exception ex)
                {
                }
            }
            FileInfo fiAdd = new FileInfo(addendum);
            if (fiAdd.LastWriteTime > lastUpdate)
            {
                unlockHandle.Set();
                waitHandle.Set();
            }
        }

        void LockFileThread()
        {
            while (!threadDone)
            {
                // Release file when requested
                waitHandle.WaitOne();
                if (File.Exists(addendum))
                {
                using (StreamReader fileStream = new StreamReader(File.Open(fromFile, FileMode.Open)))
                {
                    unlockHandle.WaitOne();
                    entries = new Dictionary<string, string>();
                    string line;
                    while ((line = fileStream.ReadLine()) != null)
                    {
                        try
                        {
                            var pair = line.Split('\t');
                            if (!entries.ContainsKey(pair[1]))
                            {
                                entries.Add(pair[0], pair[1]);
                            }
                        }
                        catch
                        { }
                    }
                }

                IList<string> newEntries = new List<string>();
                    using (StreamReader fileStream = new StreamReader(File.Open(addendum, FileMode.Open)))
                    {
                        string line;
                        while ((line = fileStream.ReadLine()) != null)
                        {
                            try
                            {
                                var pair = line.Split('\t');
                                if (!entries.ContainsKey(pair[1]))
                                {
                                    newEntries.Add(line);
                                }
                            }
                            catch
                            {
                            }
                        }
                    }

                    File.Delete(addendum);

                    using (StreamWriter fileStream = new StreamWriter(File.Open(fromFile, FileMode.Append)))
                    {
                        foreach (var line in newEntries)
                        {
                            fileStream.WriteLine(line);
                        }
                    }
                }
                copyHandle.Set();
            }
        }

        void WorkThread()
        {
            while (!threadDone)
            {
                ReplaceFile();
                StartMonitor();
                Thread.Sleep(10);
            }
        }

        public bool Start(HostControl hostControl)
        {
            Thread workerThread = new Thread(() => WorkThread());
            Thread lockThread = new Thread(() => LockFileThread());
            try
            {
                workerThread.Start();
                lockThread.Start();
            }
            catch
            {
                return false;
            }

            return true;
        }
        
        public bool Stop(HostControl hostControl)
        {
            StartMonitor();

            // Build a way to stop the program through another convoluted step
            Environment.Exit(1);
            threadDone = true;
            return true;
        }

        private void StartMonitor()
        {
            ServiceController sc = new ServiceController("HostFileReplaceMonitor");
            sc.Refresh();
            if (sc.Status != ServiceControllerStatus.Stopped)
            {
                sc.Start();
            }
        }
    }

    class Program
    {        
        static void Main(string[] args)
        {
            HostFactory.Run((configurable) =>
            configurable.Service<FileReplacer>()
            .StartAutomatically()
            .EnableServiceRecovery(r => r.RestartService(0))
            );
        }
    }
}
