using LearningPlatformAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearningPlatformAPI.Enums;
using System.Linq;

namespace LearningPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseSectionController : ControllerBase
    {
        private readonly DataContext _context;

        public CourseSectionController(DataContext context)
        {
            _context = context;
        }

        // Endpoints
        [HttpPost]
        [Route("start")]
        public ActionResult<StartSectionByUser> Start([FromBody]CreateSectionRequest request)
        {
            var result = _context.startSectionByUser(request);

            return result;
        }

        [HttpPost]
        [Route("finish")]
        public ActionResult<FinishSectionByUser> Finish([FromBody] CreateSectionRequest request)
        {
            var result = _context.finishSectionByUser(request);

            return result;

        }
    }
}
