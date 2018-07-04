using pre_registration.Models;
using Quartz;
using Quartz.Impl;
using System.Threading.Tasks;

namespace pre_registration.Jobs
{
    public class OldEmptyCuponsScheduler
    {
        public static async Task Run(string connectionString)
        {
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler scheduler = await sf.GetScheduler(); 
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<RemoveOldEmptyCupons>()
                .UsingJobData("connectionString", connectionString)
                .Build();
            
            ITrigger trigger = TriggerBuilder.Create()  // создаем триггер
                .WithIdentity("trigger1", "group1")     // идентифицируем триггер с именем и группой
                .StartNow()  // запуск сразу после начала выполнения
                .WithSimpleSchedule(x => x            // настраиваем выполнение действия
                    .WithIntervalInMinutes(30)          // через 1 минуту
                    .RepeatForever())                   // бесконечное повторение
                .Build();                               // создаем триггер

            scheduler.ScheduleJob(job, trigger);        // начинаем выполнение работы
            //return Task.CompletedTask;
        }
    }
    public class SentNotificationScheduler
    {
        public static async Task Run(string connectionString)
        {
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler scheduler = await sf.GetScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<SentNotificationJob>()
                .UsingJobData("connectionString", connectionString)
                .Build();

            ITrigger trigger = TriggerBuilder.Create()  // создаем триггер
                .WithIdentity("trigger2", "group2")     // идентифицируем триггер с именем и группой
                .StartNow()                            // запуск сразу после начала выполнения
                .WithSimpleSchedule(x => x            // настраиваем выполнение действия
                    .WithIntervalInMinutes(10)          // через 1 минуту
                    .RepeatForever())                   // бесконечное повторение
                .Build();                               // создаем триггер

            scheduler.ScheduleJob(job, trigger);        // начинаем выполнение работы
            //return Task.CompletedTask;
        }
    }
}
