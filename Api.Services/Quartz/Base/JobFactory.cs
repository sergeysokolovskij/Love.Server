using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Services.Quartz.Base
{
	public class JobFactory : IJobFactory
	{
		public IServiceProvider serviceProvider;

		public JobFactory(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}

		public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
		{
			var job = serviceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;
			return job;
		}

		public void ReturnJob(IJob job)
		{
			var disposableJob = job as IDisposable;
			disposableJob?.Dispose();
		}
	}
}
