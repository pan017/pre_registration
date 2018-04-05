using Quartz;
using Quartz.Impl;
using System.Threading.Tasks;

namespace pre_registration.Jobs
{
    public class OldEmptyCuponsScheduler
    {
        public static async Task Run()
        {
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler scheduler = await sf.GetScheduler(); 
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<RemoveOldEmptyCupons>().Build();

            ITrigger trigger = TriggerBuilder.Create()  // создаем триггер
                .WithIdentity("trigger1", "group1")     // идентифицируем триггер с именем и группой
                .StartNow()                            // запуск сразу после начала выполнения
                .WithSimpleSchedule(x => x            // настраиваем выполнение действия
                    .WithIntervalInMinutes(30)          // через 1 минуту
                    .RepeatForever())                   // бесконечное повторение
                .Build();                               // создаем триггер

            scheduler.ScheduleJob(job, trigger);        // начинаем выполнение работы
            //return Task.CompletedTask;
        }
    }
}
