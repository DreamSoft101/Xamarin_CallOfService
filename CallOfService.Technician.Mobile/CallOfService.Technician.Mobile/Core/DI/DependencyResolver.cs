using Autofac;
using Autofac.Core;

namespace CallOfService.Technician.Mobile.Core.DI
{
    public class DependencyResolver
    {
        public static IContainer Container { get; private set; }

        public static void Initialize(params IModule[] modules)
        {
            var builder = new ContainerBuilder();
            foreach (var module in modules)
            {
                builder.RegisterModule(module);
            }

            Container = builder.Build();
        }

        public static T Resolve<T>()
        {
            return Container.Resolve<T>();
        }
    }
}