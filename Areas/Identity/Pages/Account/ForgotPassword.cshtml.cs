#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace login.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender<IdentityUser> _emailSender;

        public ForgotPasswordModel(
            UserManager<IdentityUser> userManager,
            IEmailSender<IdentityUser> emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);

            // Não revelar se o e-mail existe ou não.
            if (user == null ||
                !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new
                {
                    area = "Identity",
                    code = code
                },
                protocol: Request.Scheme);

            await _emailSender.SendPasswordResetLinkAsync(
                user,
                Input.Email,
                callbackUrl!);

            return RedirectToPage("./ForgotPasswordConfirmation");
        }
    }
}