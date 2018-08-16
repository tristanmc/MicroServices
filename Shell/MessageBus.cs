//using Autofac;
//using Microsoft.ServiceFabric.Data;
//using Microsoft.ServiceFabric.Services.Communication.Runtime;
//using NServiceBus;
//using NServiceBus.Features;
//using NServiceBus.Persistence.ServiceFabric;
//using System;
//using System.Collections.Generic;
//using System.Fabric;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Shell
//{
//    class MessageBusListener : ICommunicationListener
//    {
//        private readonly IReliableStateManager _state;
//        private IEndpointInstance _endpoint;
//        private EndpointConfiguration _config;
//        private IContainer _container;

//        private StatefulServiceContext _context;

//        public MessageBusListener(IReliableStateManager state)
//        {
//            _state = state;
//        }

//        public MessageBusListener WithContext(StatefulServiceContext context)
//        {
//            _context = context;
//            return this;
//        }

//        public void Abort()
//        {
//            CloseAsync(CancellationToken.None);
//        }

//        public Task CloseAsync(CancellationToken cancellationToken)
//        {
//            return _endpoint?.Stop() ?? Task.CompletedTask;
//        }


//        public async Task RunAsync()
//        {
//            _endpoint = await Endpoint.Start(_config).ConfigureAwait(false);
//            var builder = new ContainerBuilder();
//            builder.Register(c => _endpoint).As<IMessageSession>().SingleInstance();
//            builder.Update(_container);
//        }

//        public async Task<string> OpenAsync(CancellationToken cancellationToken)
//        {
//            try
//            {
//                var endpointName = "microservices.shell";

//                _config = new EndpointConfiguration(endpointName);

//                var builder = new ContainerBuilder();
//                builder.RegisterInstance(_context);
//                _container = builder.Build();

//                _config.UseContainer<AutofacBuilder>(c => c.ExistingLifetimeScope(_container));

//                _config.Notifications.Errors.MessageSentToErrorQueue += Errors_MessageSentToErrorQueue;

//                _config.SendFailedMessagesTo("error");
//                _config.UseSerialization<JsonSerializer>();
//                _config.DisableFeature<TimeoutManager>();

//                //var rabbitmq = _config.UseTransport<RabbitMQTransport>();
//                //rabbitmq.ConnectionString(@"host=sidewinder-01.rmq.cloudamqp.com;username=lqnckota;password=pDW2_ISMD5rKKwo9FtujZ3wo9QcRadT-;virtualhost=lqnckota;");
//                //rabbitmq.TimeToWaitBeforeTriggeringCircuitBreaker(TimeSpan.FromMinutes(1));

//                // configure endpoint with state manager dependency
//                var persistence = _config.UsePersistence<ServiceFabricPersistence>();
//                persistence.StateManager(_state);

//                //var logging = LogManager.Use<DefaultFactory>();
//                //logging.Directory(@"C:\data\pros");

//                return endpointName;
//            }
//            catch (Exception e)
//            {
//                ServiceEventSource.Current.Message($"Exception starting queue: {e}");
//                return string.Empty;
//            }
//        }

//        private void Errors_MessageSentToErrorQueue(object sender, NServiceBus.Faults.FailedMessage e)
//        {
//            ServiceEventSource.Current.Message("Message sent to error queue");
//        }
//    }
//}
