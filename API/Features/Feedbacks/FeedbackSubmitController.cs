using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Feedbacks
{
    [ApiController]
    [Route("feedback")]
    [Authorize("OnlyAdmin")]
    public class FeedbackSubmitController(IFeedbackRepository feedbackRepository) : Controller
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> AddFeedback([FromBody] FeedbackSubmitDto feedbackSubmitDto)
        {
            var feedback = feedbackSubmitDto.MapToEntity();
            await feedbackRepository.AddAsync(feedback);
            return NoContent();
        }
        
        [HttpPatch("{feedbackId:int}/solve")]
        public async Task<ActionResult> SolveFeedback([FromRoute] int feedbackId)
        {
            await feedbackRepository.SolveAsync(feedbackId);
            return NoContent();
        }
        
        [HttpGet("categories")]
        [AllowAnonymous]
        public async Task<ActionResult<List<FeedbackCategoryDto>>> ReadAllCategories()
        {
            var categories = await feedbackRepository.ReadAllCategoriesAsync();
            return Ok(categories);
        }
        
        [HttpGet("search")]
        public async Task<ActionResult<FeedbacksDtoList>> PagingSearch(int pageSize, int pageNumber, bool solved)
        {
            return Ok(await feedbackRepository.PagingSearchAsync(pageSize, pageNumber, solved));
        }

        [HttpGet("unsolved-count")]
        public async Task<ActionResult<int>> CountUnsolved()
        {
            return Ok(await feedbackRepository.CountUnsolvedAsync());
        }
    }
}