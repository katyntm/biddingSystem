using CarAuction.Application.Common.Constants;
using CarAuction.Infrastructure.Jobs;
using CarAuction.Infrastructure.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Quartz;

namespace CarAuction.API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionSchedulerController : ControllerBase
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly AuctionSettingOptions _auctionSetting;

        public AuctionSchedulerController(ISchedulerFactory schedulerFactory, IOptions<AuctionSettingOptions> auctionSetting)
        {
            _schedulerFactory = schedulerFactory;
            _auctionSetting = auctionSetting.Value;
        }

        [HttpPost("start")]
        public async Task<IActionResult> ScheduleAuctionStartJob()
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var jobKey = new JobKey(QuartzConstants.Jobs.AuctionStart);
            var triggerKey = new TriggerKey(QuartzConstants.Triggers.AuctionStart);

            if (!await scheduler.CheckExists(jobKey))
            {
                var jobDetail = JobBuilder.Create<AuctionStartJob>()
                    .WithIdentity(jobKey).StoreDurably()
                    .Build();
                await scheduler.AddJob(jobDetail, true);
            }

            var trigger = TriggerBuilder.Create()
                .ForJob(jobKey)
                .WithIdentity(triggerKey)
                                //.WithCronSchedule(QuartzConstants.Cron.Every30Seconds)
                                .StartAt(_auctionSetting.AuctionSession.StartTime)
                .Build();

            var existingTrigger = await scheduler.GetTrigger(triggerKey);
            if (existingTrigger != null)
            {
                await scheduler.RescheduleJob(triggerKey, trigger);
            }
            else
            {
                await scheduler.ScheduleJob(trigger);
            }
            await scheduler.TriggerJob(jobKey);
            return Ok($"Rescheduled AuctionStartJob to run daily at {_auctionSetting.AuctionSession.StartTime.Hour}:00");

        }

        [HttpPost("end")]
        public async Task<IActionResult> ScheduleAuctionEndJob()
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var jobKey = new JobKey(QuartzConstants.Jobs.AuctionEnd);
            var triggerKey = new TriggerKey(QuartzConstants.Triggers.AuctionEnd);

            if (!await scheduler.CheckExists(jobKey))
            {
                var jobDetail = JobBuilder.Create<AuctionEndJob>()
                    .WithIdentity(jobKey).StoreDurably()
                    .Build();
                await scheduler.AddJob(jobDetail, true);
            }

            var trigger = TriggerBuilder.Create()
                .ForJob(jobKey)
                .WithIdentity(triggerKey)
                .StartAt(_auctionSetting.AuctionSession.EndTime)
                .Build();

            var existingTrigger = await scheduler.GetTrigger(triggerKey);
            if (existingTrigger != null)
            {
                await scheduler.RescheduleJob(triggerKey, trigger);
            }
            else
            {
                await scheduler.ScheduleJob(trigger);
            }
            await scheduler.TriggerJob(jobKey);
            return Ok($"Scheduled AuctionEndJob to run daily at {_auctionSetting.AuctionSession.EndTime.Hour}:00");

        }
    }
}