using System;
using Autofac;

namespace CallOfService.Technician.Mobile.Droid.Core.DI
{
	public class AndroidModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<CredentialManager>().As<ICredentialManager>().SingleInstance();
			builder.RegisterType<SqliteNet>().As<ISqLiteNet>().SingleInstance();
			builder.RegisterType<UserDialogsImpl>().As<IUserDialogs>().SingleInstance();
			builder.RegisterType<MediaPicker>().As<IMediaPicker>().SingleInstance();
			builder.RegisterType<ImageCompressor>().As<IImageCompressor>().SingleInstance();
		}
	}
}

