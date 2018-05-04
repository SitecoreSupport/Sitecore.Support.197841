using System.Collections.Generic;
using Sitecore.DependencyInjection;

namespace Sitecore.Support.DependencyInjection
{
  public abstract class BaseServiceProviderBuilder : Sitecore.DependencyInjection.BaseServiceProviderBuilder
  {
    public override IEnumerable<IServicesConfigurator> GetServicesConfigurators()
    {
      foreach (IServicesConfigurator servicesConfigurator in base.GetServicesConfigurators())
        yield return servicesConfigurator;
      yield return (IServicesConfigurator)new Sitecore.Support.DependencyInjection.ServicesScopeConfigurator();
    }
  }
}