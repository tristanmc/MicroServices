using MessageDefinitions;
using MicroServices.Current.Controllers;
using NServiceBus;
using Persistence;
using System;
using System.Threading.Tasks;

namespace MicroServices.Current.Handlers
{
    public class NewSinkHandler : IHandleMessages<NewSink>
    {
        private readonly IRepository<long, Sink> _repo;

        public NewSinkHandler(IRepository<long, Sink> repo)
        {
            _repo = repo;
        }

        public async Task Handle(NewSink message, IMessageHandlerContext context)
        {
            long key = message.Sink.Name.GetHashCode();
            await _repo.Save(key, new Sink
            {
                Model = message.Sink.Name,
                Width = new Random().Next()
            });
        }
    }
}
