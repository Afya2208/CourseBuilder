using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Dto;
using Models.Entities;

namespace API.Controllers
{
    [ApiController]
    [Route("feedback")]
    public class FeedbackSubmitController(FeedbackSubmitRepository feedbackSubmitRepository,
         ILogger<FeedbackSubmitController> logger, CoursesDbContext context) : Controller
    {
        [HttpPost]
        public async Task<IActionResult> AddComplaint([FromBody] FeedbackSubmitDto feedbackSubmitDto)
        {
            logger.LogInformation("Обращение от id={UserId}/email={Rmail}", feedbackSubmitDto.UserId,
                feedbackSubmitDto.Email);
            var res = await feedbackSubmitRepository.AddAsync(feedbackSubmitDto.ToEntity());
            logger.LogDebug("Обращение сохранено id={Id}", res.Id);
            return Ok();
        }
        
        [HttpPost("{id:int}/solve")]
        public async Task<IActionResult> SolveFeedback(int id)
        {
            var feedback = await context.FeedbackSubmits.FindAsync(id);
            feedback.DateTimeSolved = DateTime.Now;
            await context.SaveChangesAsync();
            return Ok();
        }
        [HttpGet("categories")]
        public async Task<IActionResult> FindAllCategories()
        {
            var categories = await context.FeedbackCategories
                .AsNoTracking()
                .Select(x=> new
                {
                    x.Id,
                    x.Name
                })
                .ToListAsync();
            return Ok(categories);
        }
        
        [HttpGet("paged")]
        public async Task<IActionResult> FindAllSolved(int pageSize, int pageNumber, bool solved)
        {
            var feedbackSubmits = context.FeedbackSubmits
                .AsNoTracking();
            if (solved)
            {
                feedbackSubmits = feedbackSubmits.Where(feedback => feedback.DateTimeSolved != null);
            }
            else
            {
                feedbackSubmits = feedbackSubmits.Where(feedback => feedback.DateTimeSolved == null);
            }
            var data = feedbackSubmits
                .OrderByDescending(x => x.DateTimeSent)
                .Select(x => new
                {
                    x.Id,
                    x.Email,
                    x.DateTimeSolved,
                    x.DateTimeSent,
                    x.FeedbackCategory,
                    x.FeedbackCategoryId,
                    x.Text,
                    x.Title,
                    x.UserId
                });
            var totalCount = await data.CountAsync();
            var list = await data
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();
            return Ok(new
            {
                totalCount,
                feedbackSubmits = list,
            });
        }
        [HttpGet("count")]
        public async Task<IActionResult> CountUnsolved()
        {
            var feedbackSubmits = await context.FeedbackSubmits
                .Where(feedback => feedback.DateTimeSolved == null)
                .CountAsync();
            return Ok(feedbackSubmits);
        }
    }
}