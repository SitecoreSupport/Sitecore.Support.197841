namespace Sitecore.Support.DependencyInjection
{
  using System;
  using System.Diagnostics;
  using System.Linq;
  using System.Reflection;
  using System.Web;
  using System.Web.UI;
  using Sitecore.DependencyInjection;

  /// <summary>The injection page handler factory.</summary>
  public class AutowiredPageHandlerFactory : IHttpHandlerFactory
  {
    /// <summary>The origin factory.</summary>
    internal readonly IHttpHandlerFactory OriginFactory;

    /// <summary>
    /// The service provider.
    /// </summary>
    internal readonly IServiceProvider ServiceProvider;

    /// <summary>Initializes a new instance of the <see cref="AutowiredPageHandlerFactory"/> class.</summary>
    public AutowiredPageHandlerFactory() : this(new PageHandlerFactoryWrapper(), ServiceLocator.ServiceProvider)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AutowiredPageHandlerFactory"/> class.
    /// </summary>
    /// <param name="originFactory">The origin factory.</param>
    /// <param name="serviceProvider">The service Provider.</param>
    internal AutowiredPageHandlerFactory(IHttpHandlerFactory originFactory, IServiceProvider serviceProvider)
    {
      this.OriginFactory = originFactory;
      this.ServiceProvider = serviceProvider;
    }

    /// <summary>
    /// Returns an instance of a class that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
    /// </summary>
    /// <returns>
    /// A new <see cref="T:System.Web.IHttpHandler"/> object that processes the request.
    /// </returns>
    /// <param name="context">An instance of the <see cref="T:System.Web.HttpContext"/> class that provides references to intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests. </param><param name="requestType">The HTTP data transfer method (GET or POST) that the client uses. </param><param name="url">The <see cref="P:System.Web.HttpRequest.RawUrl"/> of the requested resource. </param><param name="pathTranslated">The <see cref="P:System.Web.HttpRequest.PhysicalApplicationPath"/> to the requested resource. </param>
    public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
    {
      var httpHandler = this.OriginFactory.GetHandler(context, requestType, url, pathTranslated);
      if (httpHandler != null)
      {
        this.InitializeInstance(httpHandler);
        var page = httpHandler as Page;
        if (page != null)
        {
          page.PreLoad += (sender, args) =>
          {
            this.InitializeChildControls(page);
          };
        }
      }

      return httpHandler;
    }

    /// <summary>
    /// Enables a factory to reuse an existing handler instance.
    /// </summary>
    /// <param name="handler">The <see cref="T:System.Web.IHttpHandler"/> object to reuse. </param>
    public void ReleaseHandler(IHttpHandler handler)
    {
      this.OriginFactory.ReleaseHandler(handler);
    }

    /// <summary>Initializes child controls.</summary>
    /// <param name="control">The control.</param>
    private void InitializeChildControls(Control control)
    {
      foreach (Control childControl in control.Controls)
      {
        if (childControl is UserControl)
        {
          this.InitializeInstance(childControl);
        }

        this.InitializeChildControls(childControl);
      }
    }

    /// <summary>Initializes instance by calling dependency constructor.</summary>
    /// <param name="targetObject">The target object.</param>
    private void InitializeInstance(object targetObject)
    {
      var targetType = targetObject.GetType().BaseType;
      if (targetType.GetCustomAttribute<AllowDependencyInjectionAttribute>(false) == null)
      {
        return;
      }

      Debug.Assert(targetType != null, "targetType != null");
      var constructor = targetType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).OrderByDescending(x => x.GetParameters().Length).First();
      var parameterInfos = constructor.GetParameters();
      if (parameterInfos.Length > 0)
      {
        var resolvedParameters = parameterInfos.Select(x => ServiceLocator.ServiceProvider.GetService(x.ParameterType)).ToArray();
        constructor.Invoke(targetObject, resolvedParameters);
      }
    }

    /// <summary>The page handler factory wrapper.</summary>
    internal class PageHandlerFactoryWrapper : PageHandlerFactory
    {
    }
  }
}