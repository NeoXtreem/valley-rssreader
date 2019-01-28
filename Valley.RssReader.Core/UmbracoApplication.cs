using System.Web.Http;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Umbraco.Core;
using Umbraco.Core.Services;
using Valley.RssReader.Core.Services;
using Valley.RssReader.Core.Services.Interfaces;

public class UmbracoApplication : IApplicationEventHandler
{
    public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
    {
        var builder = new ContainerBuilder();

        // Register our controllers from this assembly.
        builder.RegisterControllers(typeof(UmbracoApplication).Assembly);
        builder.RegisterApiControllers(typeof(UmbracoApplication).Assembly);

        // Register controllers from the Umbraco assemblies.
        builder.RegisterControllers(typeof(Umbraco.Web.UmbracoApplication).Assembly);
        builder.RegisterApiControllers(typeof(Umbraco.Web.UmbracoApplication).Assembly);

        // Register the types we need to resolve.
        builder.RegisterInstance(applicationContext.Services.ContentService).As<IContentService>();
        builder.RegisterType<RssReaderService>().As<IRssReaderService>();

        // Set up MVC to use Autofac as a dependency resolver.
        IContainer container = builder.Build();
        System.Web.Mvc.DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        //GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
    }

    public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
    {
    }

    public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
    {
    }
}
