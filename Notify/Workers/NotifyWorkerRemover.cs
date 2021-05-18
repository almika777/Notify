using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Notify.Workers
{
    public class NotifyWorkerRemover : BackgroundService
    {
        public NotifyWorkerRemover()
        {
            
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return null;
        }
    }
}
