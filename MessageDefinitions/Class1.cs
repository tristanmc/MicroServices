using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageDefinitions
{
    public class NewSink : IEvent
    {
        public MessageSink Sink { get; set; }
    }

    public class MessageSink
    {
        public string Name { get; set; }
    }

    public class CreateSink : ICommand
    {
        public MessageSink Sink { get; set; }
    }
}
