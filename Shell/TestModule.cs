using Nancy;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Shell
{
    public class TestModule : NancyModule
    {
        private readonly IRepository _repo;

        public TestModule(IRepository repository)
        {
            _repo = repository;

            Get["tests", true] = GetResult;
        }

        private async Task<dynamic> GetResult(dynamic parameters, CancellationToken token)
        {
            return Response.AsJson(_repo.Test);
        }
    }
}
