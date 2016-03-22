using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using CallOfService.Technician.Mobile.Core.SystemServices;
using CallOfService.Technician.Mobile.Features.Login;
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
            //View Models
            builder.RegisterType<LoginViewModel>().AsSelf();

            //Services
            builder.RegisterType<LoginService>().As<ILoginService>().SingleInstance();
            builder.RegisterType<Logger>().As<ILogger>().SingleInstance();

            //Proxies
            builder.RegisterType<LoginProxy>().As<ILoginProxy>().SingleInstance();
            
        }
    }
}
