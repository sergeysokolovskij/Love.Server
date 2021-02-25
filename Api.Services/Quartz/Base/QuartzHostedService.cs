using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Services.Quartz.Base
{
    public enum TriggerInterval
    {
        minutes,
        seconds,
        hours
    }

    public class QuartzHostedService : IHostedService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IEnumerable<JobSchedule> jobSchedules;
        private ISchedulerFactory schedulerFactory;
        private IJobFactory jobFactory;

        public QuartzHostedService(
            IServiceProvider serviceProvider,
            IEnumerable<JobSchedule> jobSchedules)
        {
            this.serviceProvider = serviceProvider;
            this.jobSchedules = jobSchedules;
        }

        public IScheduler Scheduler { get; set; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            
            using var scope = serviceProvider.GetService<IServiceScopeFactory>().CreateScope();
            jobFactory = scope.ServiceProvider.GetRequiredService<IJobFactory>();
            schedulerFactory = scope.ServiceProvider.GetRequiredService<ISchedulerFactory>();

            Scheduler = await schedulerFactory.GetScheduler(cancellationToken);
            Scheduler.JobFactory = jobFactory;

            foreach (var jobSchedule in jobSchedules)
            {
                var job = CreateJob(jobSchedule);
                var trigger = CreateTrigger(jobSchedule);

                await Scheduler.ScheduleJob(job, trigger, cancellationToken);
            }
            await Scheduler.Start();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Scheduler.Shutdown(cancellationToken);
        }

        private IJobDetail CreateJob(JobSchedule schedule)
        {
            var jobType = schedule.JobType;

            return JobBuilder.Create(jobType)
                .WithIdentity(jobType.FullName)
                .WithDescription(jobType.Name)
                .Build();
        }
        
        private ITrigger CreateTrigger(JobSchedule schedule)
        {
            return TriggerBuilder.Create()
                .WithIdentity($"{schedule.JobType.FullName}.trigger")
                .WithSimpleSchedule(x => x.WithIntervalInMinutes(schedule.IntervalInMinutes)
                    .RepeatForever())
                .StartNow()
                .Build();
        }
    }
}