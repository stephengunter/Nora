using Autofac;
using Autofac.Core.Activators.Reflection;
using System.Reflection;
using ApplicationCore.Auth;
using ApplicationCore.DataAccess;
using Microsoft.AspNetCore.Authorization;
using ApplicationCore.Authorization;

namespace ApplicationCore.DI;

public class ApplicationCoreModule : Autofac.Module
{
   protected override void Load(ContainerBuilder builder)
   {
      builder.RegisterType<JwtFactory>().As<IJwtFactory>().SingleInstance().FindConstructorsWith(new InternalConstructorFinder());
      builder.RegisterType<JwtTokenHandler>().As<IJwtTokenHandler>().SingleInstance().FindConstructorsWith(new InternalConstructorFinder());
      builder.RegisterType<TokenFactory>().As<ITokenFactory>().SingleInstance();
      builder.RegisterType<JwtTokenValidator>().As<IJwtTokenValidator>().SingleInstance().FindConstructorsWith(new InternalConstructorFinder());
      
      builder.RegisterType<HasPermissionHandler>().As<IAuthorizationHandler>().InstancePerLifetimeScope();
           
      builder.RegisterGeneric(typeof(DefaultRepository<>)).As(typeof(IDefaultRepository<>)).InstancePerLifetimeScope();


      builder.RegisterAssemblyTypes(GetAssemblyByName("ApplicationCore"))
               .Where(t => t.Name.EndsWith("Service"))
               .AsImplementedInterfaces()
               .InstancePerLifetimeScope();
   }

   public static Assembly GetAssemblyByName(String AssemblyName) => Assembly.Load(AssemblyName);

}



public class InternalConstructorFinder : IConstructorFinder
{
   public ConstructorInfo[] FindConstructors(Type targetType)
         => targetType.GetTypeInfo().DeclaredConstructors.Where(c => !c.IsPrivate && !c.IsPublic).ToArray();
}
