using Api.Services.Quartz.Base;
using Api.Services.Quartz.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Services.Quartz
{
    public static class DiExtensions
    {
        public static void AddQuartz(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ConnectionCleanerJob>();
            serviceCollection.AddSingleton<SessionCleanerJob>();

            serviceCollection.AddSingleton<IJobFactory, JobFactory>();
            serviceCollection.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            serviceCollection.AddSingleton(new JobSchedule(typeof(ConnectionCleanerJob), 30));
        }
    }
}
