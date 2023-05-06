using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace dotnet_mvc.Helpers
{
    /*
        Локализация встреонные в Identity ошибок,
        появляемых при создании пользователя.
    */
    public class MultilanguageIdentityErrorDescriber : IdentityErrorDescriber
    {
        public MultilanguageIdentityErrorDescriber()
        {

        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError()
            {
                Code = nameof(DuplicateEmail),
                Description = string.Format("Почтовый адрес {0} уже кем-то используется.", email)
            };
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError()
            {
                Code = nameof(DuplicateUserName),
                Description = string.Format("Логин {0} уже кем-то используется.", userName)
            };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError()
            {
                Code = nameof(PasswordTooShort),
                Description = string.Format("Введенный пароль слишком короткий! Минимальная длина: {0}", length)
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError()
            {
                Code = nameof(PasswordRequiresUpper),
                Description = string.Format("Пароль должен содержать хотя бы одну заглавную букву!")
            };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError()
            {
                Code = nameof(PasswordRequiresLower),
                Description = string.Format("Пароль должен содержать хотя бы одну строчную букву!")
            };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError()
            {
                Code = nameof(PasswordRequiresDigit),
                Description = string.Format("Пароль должен содержать хотя бы одну цифру!")
            };
        }
    }
}