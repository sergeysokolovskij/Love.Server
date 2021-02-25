using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Services.Quartz.Base
{
    public class JobSchedule
    {
        public Type JobType { get; }
        public int IntervalInMinutes { get; }
        public JobSchedule(Type jobtype,
            int intervalInMinutes)
        {
            this.JobType = jobtype;
            this.IntervalInMinutes = intervalInMinutes;
        }
    }
}
