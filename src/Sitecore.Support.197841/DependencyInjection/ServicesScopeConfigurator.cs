namespace Sitecore.Support.DependencyInjection
{
  using System.Linq;
  using Microsoft.Extensions.DependencyInjection;
  using Sitecore.DependencyInjection;

  /// <summary>
  /// The services scope configurator.
  /// </summary>
  public class ServicesScopeConfigurator : IServicesConfigurator
  {
    /// <summary>
    /// Adds Scoping service to the service collection.
    /// </summary>
    /// <param name="serviceCollection">
    /// The service collection.
    /// </param>
    public void Configure(IServiceCollection serviceCollection)
    {
      if (serviceCollection.All(x => x.ServiceType != typeof(ISitecoreServiceLocatorScope)))
      {
        serviceCollection.Add(new ServiceDescriptor(typeof(ISitecoreServiceLocatorScope), typeof(Sitecore.Support.DependencyInjection.SitecoreSingletonServiceScope), ServiceLifetime.Singleton));
      }
    }
  }
}