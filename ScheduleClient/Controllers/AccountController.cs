using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using ScheduleClient.Models;
using ScheduleClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ScheduleClient.Controllers
{
    public class AccountController : Controller
    {
        //propriedade responsavel por retornar o contexto do Owin
        //(back infield) 
        private UserManager<ApplicationUser> _userManager;
        //criando a propriedade
        public UserManager<ApplicationUser> UserManager
        {
            get
            {
                if(_userManager == null)
                {
                    var contextOwin = HttpContext.GetOwinContext();
                    _userManager = contextOwin.GetUserManager<UserManager<ApplicationUser>>();
                }
                return _userManager;
            }
            set
            {
                _userManager = value;
            }
        }
        //obtendo a view
        public ActionResult Register()
        {
            return View();
        }
         //post das informações
         [HttpPost]
        public async Task<ActionResult> Register(AccountRegisterViewModel model)
        {
            if (ModelState.IsValid) //added client
            {
               
                var newUser = new ApplicationUser();

                newUser.UserName = model.UserName;
                newUser.FullName = model.FullName;
                newUser.Email = model.Email;

                //dbContext.Users.Add(newUser); -- deixando o banco e trabalhando com diretamente com o Identity
                //dbContext.SaveChanges();

                var userValidEmail = await UserManager.FindByEmailAsync(model.Email);
                var userIsExist = userValidEmail != null;

                if (userIsExist)
                    return View("AwaitConfirmation");//PROBLEMAS COM A COMUNICAÇÃO DO GMAIL

                var result = await UserManager.CreateAsync(newUser, model.Password);
                //userManager.Create(newUser, model.Password);

                if (result.Succeeded)
                {
                    //enviar e-mail de confirmação
                    await SendEmailConfirmationAsync(newUser);
                    return View("AwaitConfirmation");
                }
                else
                {
                    AddError(result);
                }

            }
            return View(model);//algo deu errado devolver o model
        }

        private async Task SendEmailConfirmationAsync(ApplicationUser user)
        {
            //gerando token gerado pelo userManager
            var token = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

            //facilitando a vida do cliente para que o mesmo não precise copiar e colar o código, e sim já ser direcionado para tal
            var linkCallBack =
                Url.Action(
                    "ConfirmEmail",
                    "Account",
                    new { userId = user.Id, token = token },
                    Request.Url.Scheme);//retorna o protocolo e valida o link

           await  UserManager.SendEmailAsync(
                user.Id,
                "Schedule Beauty - Confirmação de e-mail",
                 $"Bem-vindo ao Schedule Beauty, click aqui {linkCallBack} para confirmar seu endereço de e-mail"
               // $"Bem-vindo ao Schedule Beauty, use seu código {token} para confirmar seu endereço de e-mail"
                );
        }
        private void AddError(IdentityResult result)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);

        }
        public async Task<ActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
                return View("Error");

            var result = await UserManager.ConfirmEmailAsync(userId, token);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");
            else
                return View("Error");
        }
    }
}