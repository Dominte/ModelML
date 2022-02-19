using FIIServer.Models;
using FIIServer.Repository;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FIIServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DatabaseController : ControllerBase
    {
        private readonly FoodRepository _foodRepository;

        public DatabaseController(IMongoClient client)
        {
            _foodRepository = new FoodRepository(client);
        }

        [HttpGet]
        public async Task<IEnumerable<Food>> GetAllDatabase()
        {
            var foods = await _foodRepository.GetAll();
            return foods;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var food = await _foodRepository.GetById(id);

            if (food.Id == 0)
            {
                return NotFound();
            }

            return Ok(food);
        }

        [HttpPost]
        public async Task<IActionResult> AddFood([FromBody] Food food)
        {
            try
            {
                await _foodRepository.AddFood(food);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
