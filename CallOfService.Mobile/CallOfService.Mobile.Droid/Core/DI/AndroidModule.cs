﻿using Acr.UserDialogs;
using Autofac;
using CallOfService.Mobile.Database;
using CallOfService.Mobile.Droid.Database;
using CallOfService.Mobile.Droid.Services;

namespace CallOfService.Mobile.Droid.Core.DI
{
	public class AndroidModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			//builder.RegisterType<CredentialManager>().As<ICredentialManager>().SingleInstance();
			builder.RegisterType<SqliteNet>().As<ISqLiteNet>().SingleInstance();
			//builder.RegisterType<UserDialogsImpl>().As<IUserDialogs>().SingleInstance();
			builder.Register(c=> UserDialogs.Instance).As<IUserDialogs>().SingleInstance();
			builder.RegisterType<MediaPicker>().As<IMediaPicker>().SingleInstance();
			builder.RegisterType<ImageCompressor>().As<IImageCompressor>().SingleInstance();
		}
	}
}
