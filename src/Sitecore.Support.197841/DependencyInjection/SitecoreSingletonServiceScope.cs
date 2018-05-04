namespace Sitecore.Support.DependencyInjection
{
  using System;
  using Sitecore.DependencyInjection;
  using Microsoft.Extensions.DependencyInjection;

  /// <summary>
  /// SitecoreSingletonServiceScope class encapsulate the logic to grab service provider.
  /// </summary>
  public class SitecoreSingletonServiceScope : ISitecoreServiceLocatorScope, IServiceScope, IDisposable
  {
    /// <summary>
    /// default service provider
    /// </summary>
    private IServiceProvider applicationServiceProvider;

    /// <summary>
    /// Get the service provider in scope
    /// </summary>
    public IServiceProvider ServiceProvider
    {
      get
      {
        var scope = Sitecore.Support.DependencyInjection.SitecorePerRequestScopeModule.GetScope(this.applicationServiceProvider);

        if (scope == null)
        {
          return this.applicationServiceProvider;
        }

        return scope.ServiceProvider;
      }
    }

    /// <summary>
    /// Constructor of SitecoreSingletonServiceScope
    /// </summary>
    /// <param name="applicationServiceProvider"></param>
    public SitecoreSingletonServiceScope(IServiceProvider applicationServiceProvider)
    {
      this.applicationServiceProvider = applicationServiceProvider;
    }

    /// <summary>
    /// Dispose class
    /// </summary>
    public void Dispose()
    {
    }
  }
}