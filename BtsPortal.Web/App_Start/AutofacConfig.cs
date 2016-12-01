using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using BtsPortal.Cache;
using BtsPortal.Repositories.Interface;
using BtsPortal.Web.Repository;

namespace BtsPortal.Web
{
    public static class AutofacConfig
    {
        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<HttpCacheProvider>().As<ICacheProvider>();
            builder.RegisterType<CachedBizTalkMgmtRepository>().As<IBizTalkMgmtRepository>().InstancePerRequest();
            builder.RegisterType<CachedBizTalkMsgBoxRepository>().As<IBizTalkMsgBoxRepository>().InstancePerRequest();
            builder.RegisterType<CachedBizTalkRepository>().As<IBizTalkRepository>().InstancePerRequest();
            builder.RegisterType<CachedSsoDbRepository>().As<ISsoDbRepository>().InstancePerRequest();
            builder.RegisterType<CachedEsbExceptionDbRepository>().As<IEsbExceptionDbRepository>().InstancePerRequest();
            builder.RegisterType<CachedBamRepository>().As<IBamRepository>().InstancePerRequest();
            builder.RegisterType<CachedBtsPortalRepository>().As<IBtsPortalRepository>().InstancePerRequest();


            // make controllers use constructor injection
            builder.RegisterControllers(Assembly.GetExecutingAssembly());


            //builder.RegisterModule(new ConfigurationSettingsReader("autofac"));

            // change the MVC dependency resolver to use Autofac
            DependencyResolver.SetResolver(new AutofacDependencyResolver(builder.Build()));
        }
    }
}