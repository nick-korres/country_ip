using Quartz;
using Quartz.Impl;
using CountryIP.Controllers;
using Microsoft.Extensions.Caching.Memory;

namespace CountryIP.Helpers
{
    public class TimedTaskScheduler
    {

        public static  async Task InitTask(Int32 seconds)
        {
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

            await scheduler.Start();


            
            IJobDetail job = JobBuilder.Create<UpdateIpAddressesJob>()
                .WithIdentity("job1", "group1")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(seconds)
                .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);
            // await scheduler.Shutdown();

        }
    
        
        public class UpdateIpAddressesJob : IJob
        {   
            public async Task Execute(IJobExecutionContext context)
            {
                System.Console.WriteLine("Running Task");
                if(CountriesController._cache !=null)
                {
                    System.Console.WriteLine("cache initialized");
                    var k = new UpdateIpAddressesHelper(CountriesController._cache);
                    await k.UpdateIpAddresses();
                }
                
            }
        }
    }
}

