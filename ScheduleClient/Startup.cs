using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Owin;
using ScheduleClient.App_Start.Identity;
using ScheduleClient.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

[assembly: OwinStartup(typeof(ScheduleClient.Startup))]//laço de inicialiazação com o Owin
namespace ScheduleClient
{
    public class Startup
    {
        public void Configuration(IAppBuilder builder)//pipeline de construção dessa aplicação
        {
            builder.CreatePerOwinContext<DbContext>(
                () => new IdentityDbContext<ApplicationUser>("DefaultConnection"));

            builder.CreatePerOwinContext<IUserStore<ApplicationUser>>(
                (options, contextOwin) =>
                {
                    var dbContext = contextOwin.Get<DbContext>();
                    return new UserStore<ApplicationUser>(dbContext);
                });

            builder.CreatePerOwinContext<UserManager<ApplicationUser>>(
                (options, contextOwin) =>
                {
                    var userStore = contextOwin.Get<IUserStore<ApplicationUser>>();
                    var userManager = new UserManager<ApplicationUser>(userStore);

                    var userValidator = new UserValidator<ApplicationUser>(userManager);
                    userValidator.RequireUniqueEmail = true;

                    userManager.UserValidator = userValidator;
                    //property initializer
                    userManager.PasswordValidator = new PasswordValid()
                    {
                        SizeRequired = 6,
                        SpecialCharactersRequired = true,
                        DigitsRequired = true,
                        LowerCaseRequired = true,
                        UperCaseRequired = true
                    };
                    //plugando o serviço de email para envios
                    userManager.EmailService = new EmailServiceFromClient();

                    //criando token através de options
                    var dataProtectionProvider = options.DataProtectionProvider;
                    var dataProtectionProviderCreated = dataProtectionProvider.Create("Scheduleclient");

                    userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProviderCreated);

                    return userManager;
                });
        }
    }
}