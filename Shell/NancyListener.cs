using Microsoft.ServiceFabric.Services.Communication.Runtime;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Fabric;
using Nancy.Bootstrappers.Autofac;
using Autofac;
using Persistence;
using Microsoft.ServiceFabric.Data;
using Nancy.Hosting.Self;
using Nancy.Bootstrapper;
using Nancy;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Persistence.ServiceFabric;

namespace Shell
{
    public class NancyListener : ICommunicationListener
    {
        private readonly StatefulServiceContext _context;
        private NancyHost _host;
        private NancyBootstrapper _bootstrapper;
        private string _uri;

        public NancyListener(StatefulServiceContext context)
        {
            _context = context;
        }

        public void Abort()
        {
            
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            _host?.Stop();
            return Task.CompletedTask;
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {

            var endpoint = _context.CodePackageActivationContext.GetEndpoint("ServiceEndpoint");
            int port = endpoint.Port;

            //_listeningAt = $@"http://+:{port}/";

            _uri = $@"http://localhost:{port}";
            _bootstrapper = new NancyBootstrapper(_context);
            _host = new NancyHost(new Uri(_uri), _bootstrapper);

            //_publishAt = _listeningAt.Replace("+", FabricRuntime.GetNodeContext().IPAddressOrFQDN);

            return Task.FromResult(_uri);
        }

        public Task RunAsync(IReliableStateManager stateManager)
        {
            ServiceEventSource.Current.Message($"Starting nancy on {_uri}");
            _bootstrapper.WithStateManager(stateManager);
            _host.Start(); 

            return Task.CompletedTask;
        }

        

    }

    public class NancyBootstrapper : AutofacNancyBootstrapper
    {
        private readonly StatefulServiceContext _context;
        private IReliableStateManager _state;
        private EndpointConfiguration _config;
        //private IEndpointInstance _endpoint;

        public NancyBootstrapper(StatefulServiceContext context)
        {
            _context = context;
        }

        public void WithStateManager(IReliableStateManager state)
        {
            var persistence = _config.UsePersistence<ServiceFabricPersistence>();
            persistence.StateManager(_state);
            _state = state;
        }

        
        protected override void ConfigureApplicationContainer(ILifetimeScope existingContainer)
        {
            _config = CreateEndpointConfiguration(existingContainer);
            //_endpoint = ;
            existingContainer.Update(Register);
        }

        private void Register(ContainerBuilder builder)
        {
            builder.Register(c => new Repository<string, Kitchen>(_state)).As<IRepository<string, Kitchen>>();
            builder.Register(c => Endpoint.Start(_config).ConfigureAwait(false).GetAwaiter().GetResult()).As<IMessageSession>().SingleInstance();
        }

        private EndpointConfiguration CreateEndpointConfiguration(ILifetimeScope container)
        {
            try
            {
                var endpointName = "microservices.shell";

                var config = new EndpointConfiguration(endpointName);

                config.UseContainer<AutofacBuilder>(c => c.ExistingLifetimeScope(container));

                config.Notifications.Errors.MessageSentToErrorQueue += Errors_MessageSentToErrorQueue;

                config.SendFailedMessagesTo("error");
                config.UseSerialization<JsonSerializer>();
                config.DisableFeature<TimeoutManager>();
                config.DisableFeature<MessageDrivenSubscriptions>();

                //var rabbitmq = _config.UseTransport<RabbitMQTransport>();
                //rabbitmq.ConnectionString(@"host=sidewinder-01.rmq.cloudamqp.com;username=lqnckota;password=pDW2_ISMD5rKKwo9FtujZ3wo9QcRadT-;virtualhost=lqnckota;");
                //rabbitmq.TimeToWaitBeforeTriggeringCircuitBreaker(TimeSpan.FromMinutes(1));

                // configure endpoint with state manager dependency
                

                //var logging = LogManager.Use<DefaultFactory>();
                //logging.Directory(@"C:\data\pros");

                return config;
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.Message($"Exception starting queue: {e}");
                return null;
            }
        }

        private void Errors_MessageSentToErrorQueue(object sender, NServiceBus.Faults.FailedMessage e)
        {
            ServiceEventSource.Current.Message("Message sent to error queue");
        }
    }
}
