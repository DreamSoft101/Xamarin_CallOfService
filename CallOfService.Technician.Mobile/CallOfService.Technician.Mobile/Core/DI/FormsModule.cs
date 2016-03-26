using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using CallOfService.Technician.Mobile.Core.Security;
using CallOfService.Technician.Mobile.Core.SystemServices;
using CallOfService.Technician.Mobile.Database;
using CallOfService.Technician.Mobile.Database.Repos;
using CallOfService.Technician.Mobile.Database.Repos.Abstracts;
using CallOfService.Technician.Mobile.Domain;
using CallOfService.Technician.Mobile.Features.Login;
using CallOfService.Technician.Mobile.Features.Welcome;
using CallOfService.Technician.Mobile.Proxies;
using CallOfService.Technician.Mobile.Proxies.Abstratcs;
using CallOfService.Technician.Mobile.Services;
using CallOfService.Technician.Mobile.Services.Abstracts;

namespace CallOfService.Technician.Mobile.Core.DI
{
    public class FormsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegisterViewModels(builder);
            RegisterServces(builder);
            RegisterDatabaseDbSets(builder);
            RegisterRepos(builder);
            RegisterProxies(builder);
        }

        private void RegisterProxies(ContainerBuilder builder)
        {
            builder.RegisterType<LoginProxy>().As<ILoginProxy>().SingleInstance();
            builder.RegisterType<AppointmentProxy>().As<IAppointmentProxy>().SingleInstance();
        }

        private void RegisterRepos(ContainerBuilder builder)
        {
            builder.RegisterType<UserRepo>().As<IUserRepo>().SingleInstance();
        }

        private void RegisterDatabaseDbSets(ContainerBuilder builder)
        {
            builder.RegisterType<DbInitializer>().AsSelf().SingleInstance();
            builder.RegisterType<DbSet<UserProfile>>().As<IDbSet<UserProfile>>().SingleInstance();
        }

        private void RegisterServces(ContainerBuilder builder)
        {
            builder.RegisterType<LoginService>().As<ILoginService>().SingleInstance();
            builder.RegisterType<Logger>().As<ILogger>().SingleInstance();
            builder.RegisterType<AppointmentService>().As<IAppointmentService>().SingleInstance();
        }

        private void RegisterViewModels(ContainerBuilder builder)
        {
            builder.RegisterType<LoginViewModel>().AsSelf();
            builder.RegisterType<WelcomeViewModel>().AsSelf();
        }
    }
}
