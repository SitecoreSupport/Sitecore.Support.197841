using System.Collections.Generic;
using Sitecore.Configuration;
using Sitecore.DependencyInjection;

namespace Sitecore.Support.DependencyInjection
{
  public abstract class BaseServiceProviderBuilder : Sitecore.DependencyInjection.BaseServiceProviderBuilder
  {
    public override IEnumerable<IServicesConfigurator> GetServicesConfigurators()
    {
      var configuration = ConfigReader.GetConfiguration();
      yield return new ServicesConfigurator(configuration);
      yield return new DefaultSitecoreServicesConfigurator();
      yield return new ConfiguratorsConfigurator(configuration);
      yield return new ServicesScopeConfigurator();
    }
  }
}