using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proiect.Core.Entities;
using Proiect.Core.Interfaces;

namespace Proiect.Web.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private IRepository _repository;

        public CategoryController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Route("getAll")]
        public async Task<IActionResult> GetAllArticles()
        {
            var categories = await _repository.ListAsync<Category>();
            return new JsonResult(categories);
        }
    }
}
