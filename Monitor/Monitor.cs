using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Topshelf;

namespace Monitor
{
		public class HostFileReplaceMonitor : ServiceControl
		{
				public bool Start(HostControl hostControl)
				{
						Thread workerThread = new Thread(() => WorkThread());
						try
						{
								workerThread.Start();
								return true;
						}
						catch
						{
								return false;
						}
				}

				private void WorkThread()
				{
						ServiceController sc = new ServiceController("HostFileReplace");
						sc.WaitForStatus(ServiceControllerStatus.Stopped);
						sc.Start();
				}

				public bool Stop(HostControl hostControl)
				{
						ServiceController sc = new ServiceController("HostFileReplace");
						sc.WaitForStatus(ServiceControllerStatus.Stopped);
						sc.Start();
						return true;
				}
		}
}
