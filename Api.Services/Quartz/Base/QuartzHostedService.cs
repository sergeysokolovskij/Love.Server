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
        private readonly IJobFactory jobFactory;
        private readonly IEnumerable<JobSchedule> jobSchedules;
        private readonly ISchedulerFactory schedulerFactory;

        public QuartzHostedService(
            IJobFactory jobFactory,
            ISchedulerFactory schedulerFactory,
            IEnumerable<JobSchedule> jobSchedules)
        {
            this.jobFactory = jobFactory;
            this.jobSchedules = jobSchedules;
            this.schedulerFactory = schedulerFactory;
        }

        public IScheduler Scheduler { get; set; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
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