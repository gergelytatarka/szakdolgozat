using Microsoft.AspNetCore.Identity;

namespace CaseHandler.WebApplication.Configuration
{
    public class HungarianIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateEmail(string email)
        {
            return ReturnIdentityError(nameof(DuplicateEmail), "A megadott email cím már regisztrálva van.");
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return ReturnIdentityError(nameof(DuplicateUserName), "A megadott felhasználónév már regisztrálva van.");
        }

        public override IdentityError InvalidEmail(string email)
        {
            return ReturnIdentityError(nameof(InvalidEmail), "A megadott email cím formátuma nem megfelelő.");
        }

        public override IdentityError InvalidUserName(string userName)
        {
            return ReturnIdentityError(nameof(InvalidUserName), "A megadott felhasználónév formátuma nem megfelelő.");
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return ReturnIdentityError(nameof(PasswordRequiresDigit), "A jelszónak számot is kell tartalmaznia.");
        }

        public override IdentityError PasswordRequiresLower()
        {
            return ReturnIdentityError(nameof(PasswordRequiresLower), "A jelszónak kisbetűs karaktert is kell tartalmaznia.");
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return ReturnIdentityError(nameof(PasswordRequiresUpper), "A jelszónak nagybetűs karaktert is kell tartalmaznia.");
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return ReturnIdentityError(nameof(PasswordRequiresNonAlphanumeric), "A jelszónak speciális karaktert is kell tartalmaznia.");
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return ReturnIdentityError(nameof(PasswordTooShort), "A megadott jelszó túl rövid.");
        }

        private IdentityError ReturnIdentityError(string code, string desciption)
        {
            return new IdentityError
            {
                Code = code,
                Description = desciption
            };
        }
    }
}
