using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Persistence;

namespace MicroServices.Current.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IRepository<long, Sink> _repo;

        public ValuesController(IRepository<long, Sink> repo)
        {
            _repo = repo;
        }

        // GET api/values
        [HttpGet]
        public async Task<IEnumerable<Sink>> Get()
        {

            return await _repo.GetAll();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<Sink> Get(long id)
        {
            return await _repo.Get(id);
        }

        // POST api/values
        [HttpPost]
        public async Task Post([FromBody]Sink value)
        {
            long key = value.Model.GetHashCode();
            await _repo.Save(key, value);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task Put(long id, [FromBody]Sink value)
        {
            await _repo.Save(id, value);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(long id)
        {
        }
    }

    public class Sink
    {
        public int Width { get; set; }

        public string Model { get; set; }
    }
}
