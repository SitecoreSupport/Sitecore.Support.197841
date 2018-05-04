namespace Sitecore.Support.DependencyInjection
{
  using System;
  using System.Web;
  using Sitecore.DependencyInjection;
  using Microsoft.Extensions.DependencyInjection;
  /// <summary>
  /// The per request service scope provider.
  /// </summary>
  public class SitecorePerRequestScopeModule : IHttpModule
  {
    /// <summary>
    /// The scope key.
    /// </summary>
    internal static readonly Type ScopeKey = typeof(ISitecoreServiceLocatorScope);

    /// <summary>
    /// The service provider.
    /// </summary>
    internal readonly IServiceProvider ServiceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecorePerRequestScopeModule"/> class.
    /// </summary>
    internal SitecorePerRequestScopeModule()
      : this(ServiceLocator.ServiceProvider)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecorePerRequestScopeModule"/> class. 
    /// </summary>
    /// <param name="serviceProvider">The service Provider.</param>
    internal SitecorePerRequestScopeModule(IServiceProvider serviceProvider)
    {
      this.ServiceProvider = serviceProvider;
    }

    /// <summary>
    /// Gets scope the scope.
    /// </summary>
    /// <param name="serviceProvider">The service Provider.</param>
    /// <returns>The <see cref="IServiceScope"/>.</returns>
    public static ISitecoreServiceLocatorScope GetScope(IServiceProvider serviceProvider)
    {
      var httpContext = serviceProvider.GetService<HttpContextBase>();
      if (httpContext == null)
      {
        return null;
      }

      return httpContext.Items[ScopeKey] as ISitecoreServiceLocatorScope;
    }

    /// <summary>Initializes a module and prepares it to handle requests.</summary>
    /// <param name="context">An <see cref="T:System.Web.HttpApplication" /> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application.</param>
    public void Init(HttpApplication context)
    {
      context.BeginRequest += (sender, e) =>
      {
        this.BeginRequest();
      };

      context.EndRequest += (sender, e) =>
      {
        this.EndRequest();
      };
    }

    /// <summary>Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule" />.</summary>
    public void Dispose()
    {
    }

    /// <summary>
    /// The begin request.
    /// </summary>
    private void BeginRequest()
    {
      var httpContext = this.ServiceProvider.GetRequiredService<HttpContextBase>();
      var serviceScopeFactory = this.ServiceProvider.GetRequiredService<IServiceScopeFactory>();

      httpContext.Items[ScopeKey] = new SitecoreServiceLocatorScope(serviceScopeFactory.CreateScope());
    }

    /// <summary>
    /// The end request.
    /// </summary>
    private void EndRequest()
    {
      var httpContext = this.ServiceProvider.GetRequiredService<HttpContextBase>();

      var scope = httpContext.Items[ScopeKey] as ISitecoreServiceLocatorScope;
      if (scope != null)
      {
        scope.Dispose();
        httpContext.Items.Remove(ScopeKey);
      }
    }

    /// <summary>
    /// The service locator scope.
    /// </summary>
    internal class SitecoreServiceLocatorScope : ISitecoreServiceLocatorScope
    {
      /// <summary>
      /// The service scope.
      /// </summary>
      internal readonly IServiceScope ServiceScope;

      /// <summary>
      /// Initializes a new instance of the <see cref="SitecoreServiceLocatorScope"/> class.
      /// </summary>
      /// <param name="serviceScope">The service Scope.</param>
      public SitecoreServiceLocatorScope(IServiceScope serviceScope)
      {
        this.ServiceScope = serviceScope;
      }

      /// <summary>
      /// Gets the <see cref="T:System.IServiceProvider" /> used to resolve dependencies from the scope.
      /// </summary>
      public IServiceProvider ServiceProvider
      {
        get
        {
          return this.ServiceScope.ServiceProvider;
        }
      }

      /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
      public void Dispose()
      {
        this.ServiceScope.Dispose();
      }
    }
  }
}