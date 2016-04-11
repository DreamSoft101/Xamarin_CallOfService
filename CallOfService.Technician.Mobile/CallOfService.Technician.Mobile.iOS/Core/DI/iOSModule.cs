using Acr.UserDialogs;
using Autofac;
using CallOfService.Technician.Mobile.Core.Security;
using CallOfService.Technician.Mobile.Database;
using CallOfService.Technician.Mobile.iOS.Core.Security;
using CallOfService.Technician.Mobile.iOS.Database;

namespace CallOfService.Technician.Mobile.iOS.Core.DI
{
    public class IosModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CredentialManager>().As<ICredentialManager>().SingleInstance();
            builder.RegisterType<SqliteNet>().As<ISqLiteNet>().SingleInstance();
            builder.RegisterType<UserDialogsImpl>().As<IUserDialogs>().SingleInstance();
        }
    }
}