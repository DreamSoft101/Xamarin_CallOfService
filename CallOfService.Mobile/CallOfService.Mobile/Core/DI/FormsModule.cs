using Autofac;
using CallOfService.Mobile.Core.Security;
using CallOfService.Mobile.Core.SystemServices;
using CallOfService.Mobile.Database;
using CallOfService.Mobile.Database.Repos;
using CallOfService.Mobile.Database.Repos.Abstracts;
using CallOfService.Mobile.Domain;
using CallOfService.Mobile.Features.Calendar;
using CallOfService.Mobile.Features.JobDetails;
using CallOfService.Mobile.Features.Jobs;
using CallOfService.Mobile.Features.Login;
using CallOfService.Mobile.Features.Welcome;
using CallOfService.Mobile.Proxies;
using CallOfService.Mobile.Proxies.Abstratcs;
using CallOfService.Mobile.Services;
using CallOfService.Mobile.Services.Abstracts;

namespace CallOfService.Mobile.Core.DI
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
            builder.RegisterType<AppointmentRepo>().As<IAppointmentRepo>().SingleInstance();
        }

        private void RegisterDatabaseDbSets(ContainerBuilder builder)
        {
            builder.RegisterType<DbInitializer>().AsSelf().SingleInstance();
            builder.RegisterType<DbSet<UserProfile>>().As<IDbSet<UserProfile>>().SingleInstance();
            builder.RegisterType<DbSet<Appointment>>().As<IDbSet<Appointment>>().SingleInstance();
            builder.RegisterType<DbSet<JobDetails>>().As<IDbSet<JobDetails>>().SingleInstance();
        }

        private void RegisterServces(ContainerBuilder builder)
        {
            builder.RegisterType<LoginService>().As<ILoginService>().SingleInstance();
            builder.RegisterType<Logger>().As<ILogger>().SingleInstance();
            builder.RegisterType<AppointmentService>().As<IAppointmentService>().SingleInstance();
			builder.RegisterType<UserService>().As<IUserService>().SingleInstance();
            builder.RegisterType<CredentialManager>().As<ICredentialManager>().SingleInstance();
        }

        private void RegisterViewModels(ContainerBuilder builder)
        {
            builder.RegisterType<LoginViewModel>().AsSelf();
            builder.RegisterType<WelcomeViewModel>().AsSelf();
            builder.RegisterType<JobsViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<CalendarViewModel>().AsSelf().SingleInstance();
			builder.RegisterType<JobDetailsViewModel> ().AsSelf ().SingleInstance ();
			builder.RegisterType<NoteViewModel> ().AsSelf ();
			builder.RegisterType<MainMenuViewModel> ().AsSelf ();

        }
    }
}
