using Acr.UserDialogs;
using Autofac;
using CallOfService.Mobile.Core.Security;
using CallOfService.Mobile.Database;
using CallOfService.Mobile.iOS.Database;
using CallOfService.Mobile.iOS.Services;

namespace CallOfService.Mobile.iOS.Core.DI
{
    public class IosModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CredentialManager>().As<ICredentialManager>().SingleInstance();
            builder.RegisterType<SqliteNet>().As<ISqLiteNet>().SingleInstance();
            builder.RegisterType<UserDialogsImpl>().As<IUserDialogs>().SingleInstance();
			builder.RegisterType<MediaPicker> ().As<IMediaPicker> ().SingleInstance ();
			builder.RegisterType<ImageCompressor> ().As<IImageCompressor> ().SingleInstance ();
        }
    }
}