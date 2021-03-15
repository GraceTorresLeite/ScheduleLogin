using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ScheduleClient.ViewModels
{
    public class AccountRegisterViewModel
    {
        [Required]
        [Display(Name ="Nome")]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "Nome Completo")] //display serve para alterar conteudo na tela
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }
    }
}