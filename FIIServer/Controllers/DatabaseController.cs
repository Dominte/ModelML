using System.Collections.Generic;
using System.Threading.Tasks;
using FIIServer.Models;
using FIIServer.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FIIServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DatabaseController : ControllerBase
    {
        private readonly FoodRepository _foodRepository = new FoodRepository();

        [HttpGet]
        public async Task<IEnumerable<Food>> GetAllDatabase()
        {
            var foods = await _foodRepository.GetAll();
            return foods;
        }
    }
}
