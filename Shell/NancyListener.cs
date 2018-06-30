using Microsoft.ServiceFabric.Services.Communication.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Fabric;
using Microsoft.Owin.Hosting;
using Owin;
using Nancy.Bootstrapper;
using Nancy;
using Nancy.Bootstrappers.Autofac;
using Autofac;
using Persistence;

namespace Shell
{
    public class NancyListener : ICommunicationListener
    {
        private IDisposable _listener;

        private readonly StatefulServiceContext _context;

        public NancyListener(StatefulServiceContext context)
        {
            _context = context;
        }

        public void Abort()
        {
            
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            _listener?.Dispose();
            return Task.CompletedTask;
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {

            var endpoint = _context.CodePackageActivationContext.GetEndpoint("ServiceEndpoint");
            int port = endpoint.Port;

            var listeningAt = $@"http://+:{port}/";

            var opts = new StartOptions($@"http://*:{port}");
            _listener = WebApp.Start<Startup>(listeningAt);

            var publishAt = listeningAt.Replace("+", FabricRuntime.GetNodeContext().IPAddressOrFQDN);

            ServiceEventSource.Current.Message($"Starting owin on {publishAt}");

            return Task.FromResult(publishAt);
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseNancy();
        }
    }

    public class NancyBootstrapper : AutofacNancyBootstrapper
    {

        protected override void ConfigureApplicationContainer(ILifetimeScope existingContainer)
        {
            existingContainer.Update(b => b.RegisterType<Repository>().As<IRepository>());
            base.ConfigureApplicationContainer(existingContainer);
        }
    }
}
