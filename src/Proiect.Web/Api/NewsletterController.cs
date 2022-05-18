using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Proiect.Core.Entities;
using Proiect.Core.Interfaces;
using Proiect.Web.ApiModels;

namespace Proiect.Web.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsletterController : ControllerBase
    {
        private IRepository _repository;
        public NewsletterController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> AddNewsletterSubscription([FromBody] NewsletterRequestDTO newsletterModel)
        {
            var entity = new NewsletterSubscription
            {
                EmailAddress = newsletterModel.Email
            };

            try
            {
                await _repository.AddAsync(entity);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok();
        }
    }
}
