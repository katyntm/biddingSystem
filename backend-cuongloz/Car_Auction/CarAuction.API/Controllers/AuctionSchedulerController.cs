using Microsoft.AspNetCore.Mvc;
using Quartz;
using System.Text.Json;
using CarAuction.Infrastructure.Jobs;

namespace CarAuction.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionSchedulerController : ControllerBase
    {
        private readonly ISchedulerFactory _schedulerFactory;

        public AuctionSchedulerController(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }

        [HttpPost("start")]
        public async Task<IActionResult> ScheduleAuctionStartJob()
        {
            var filePath = "C:\\Users\\CuongPC10\\Desktop\\OJT_Training\\backend\\Car_Auction\\CarAuction.Infrastructure\\LoadData\\auctionSetting.json";
            if (!System.IO.File.Exists(filePath))
                return NotFound($"auctionSetting.json not found at {filePath}");

            var json = await System.IO.File.ReadAllTextAsync(filePath);
            var root = JsonDocument.Parse(json).RootElement;
            var startTime = root.GetProperty("auctionSession").GetProperty("startTime").GetDateTime();

            var scheduler = await _schedulerFactory.GetScheduler();
            var jobKey = new JobKey("AuctionStartJob");
            var triggerKey = new TriggerKey("AuctionStartJobTrigger");

            if (!await scheduler.CheckExists(jobKey))
            {
                var jobDetail = JobBuilder.Create<AuctionStartJob>()
                    .WithIdentity(jobKey)
                    .Build();
                await scheduler.AddJob(jobDetail, true);
            }

            var trigger = TriggerBuilder.Create()
                .ForJob(jobKey)
                .WithIdentity(triggerKey)
                //.WithCronSchedule($"0 0 {startTime.Hour} * * ?")
                .WithCronSchedule($"0/5 * * * * ?")
                .Build();

            var existingTrigger = await scheduler.GetTrigger(triggerKey);
            if (existingTrigger != null)
            {
                await scheduler.RescheduleJob(triggerKey, trigger);
                return Ok($"Rescheduled AuctionStartJob to run daily at {startTime.Hour}:00");
            }
            else
            {
                await scheduler.ScheduleJob(trigger);
                return Ok($"Scheduled AuctionStartJob to run daily at {startTime.Hour}:00");
            }
        }

        [HttpPost("end")]
        public async Task<IActionResult> ScheduleAuctionEndJob()
        {
            var filePath = "C:\\Users\\CuongPC10\\Desktop\\OJT_Training\\backend\\Car_Auction\\CarAuction.Infrastructure\\LoadData\\auctionSetting.json";
            if (!System.IO.File.Exists(filePath))
                return NotFound($"auctionSetting.json not found at {filePath}");

            var json = await System.IO.File.ReadAllTextAsync(filePath);
            var root = JsonDocument.Parse(json).RootElement;
            var endTime = root.GetProperty("auctionSession").GetProperty("endTime").GetDateTime();

            var scheduler = await _schedulerFactory.GetScheduler();
            var jobKey = new JobKey("AuctionEndJob");
            var triggerKey = new TriggerKey("AuctionEndJobTrigger");

            if (!await scheduler.CheckExists(jobKey))
            {
                var jobDetail = JobBuilder.Create<AuctionEndJob>()
                    .WithIdentity(jobKey)
                    .Build();
                await scheduler.AddJob(jobDetail, true);
            }

            var trigger = TriggerBuilder.Create()
                .ForJob(jobKey)
                .WithIdentity(triggerKey)
                //.WithCronSchedule($"0 0 {endTime.Hour} * * ?")
                .WithCronSchedule($"0/5 * * * * ?")
                .Build();

            var existingTrigger = await scheduler.GetTrigger(triggerKey);
            if (existingTrigger != null)
            {
                await scheduler.RescheduleJob(triggerKey, trigger);
                return Ok($"Rescheduled AuctionEndJob to run daily at {endTime.Hour}:00");
            }
            else
            {
                await scheduler.ScheduleJob(trigger);
                return Ok($"Scheduled AuctionEndJob to run daily at {endTime.Hour}:00");
            }
        }
    }
}