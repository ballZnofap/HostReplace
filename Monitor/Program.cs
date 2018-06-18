using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace Monitor
{
		class Program
		{
				static void Main(string[] args)
				{
						HostFactory.Run((configurable) =>
						{
						 configurable.Service<HostFileReplaceMonitor>()
						.StartAutomatically()
						.EnableServiceRecovery(r => r.RestartService(0));
						 configurable.SetServiceName("HostFileReplaceMonitor");
						}
					 );
				}
		}
}
