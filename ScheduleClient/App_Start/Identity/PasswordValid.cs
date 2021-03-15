using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace ScheduleClient.App_Start.Identity
{
    public class PasswordValid : IIdentityValidator<string>
    {
        public int SizeRequired { get; set; }

        public bool SpecialCharactersRequired { get; set; }

        public bool LowerCaseRequired { get; set; }

        public bool UperCaseRequired { get; set; }

        public bool DigitsRequired { get; set; }

        public async Task<IdentityResult> ValidateAsync(string item)
        {
            var errors = new List<string>();

            if (!VerifySize(item))           
                errors.Add($"A senha deve conter no mínimo {SizeRequired}");

            if (!VerifySpecialCharacters(item))
                errors.Add($"A senha deve conter caracteres especiais");

            if (!VerifyUpperCase(item))
                errors.Add($"A senha deve conter Letra Maiúscula");

            if (!VerifyLowerCase(item))
                errors.Add($"A senha deve conter Letra Minúscula");

            if (!VerifyDigits(item))
                errors.Add($"A senha deve conter dígitos");

            if (errors.Any())
                return IdentityResult.Failed(errors.ToArray());
            else
                return IdentityResult.Success;
        }

        private bool VerifySize(string password) => password?.Length >= SizeRequired;

        private bool VerifySpecialCharacters(string password) => Regex.IsMatch(password, @"[~`!@#$%^&*()+=|\\{}':;.,<>/?[\]""_-]");

        private bool VerifyLowerCase(string password) => password.Any(char.IsLower);

        private bool VerifyUpperCase(string password) => password.Any(char.IsUpper);

        private bool VerifyDigits(string password) => password.Any(char.IsDigit);

    }
}