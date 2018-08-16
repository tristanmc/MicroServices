using MessageDefinitions;
using Nancy;
using NServiceBus;
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
        private readonly IRepository<string, Kitchen> _repo;
        private readonly IMessageSession _endpoint;

        public TestModule(IRepository<string, Kitchen> repository, IMessageSession endpoint)
        {
            _repo = repository;
            _endpoint = endpoint;


            Get["kitchens", true] = GetAll;
            Post["kitchens", true] = Create;
        }

        private async Task<dynamic> GetAll(dynamic parameters, CancellationToken token)
        {
            return Response.AsJson(await _repo.GetAll());
        }

        private async Task<dynamic> Create(dynamic parameters, CancellationToken token)
        {
            string name = parameters["name"];

            Kitchen kitchen = new Kitchen
            {
                Name = name,
                ShelfCount = 3
            };

            await _endpoint.Publish(new NewSink { Sink = new MessageSink { Name = name } });

            return Response.AsJson(await _repo.Save(name, kitchen));
        }
    }

    public class Kitchen
    {
        public int ShelfCount { get; set; }
        public string Name { get; set; }
    }
}
