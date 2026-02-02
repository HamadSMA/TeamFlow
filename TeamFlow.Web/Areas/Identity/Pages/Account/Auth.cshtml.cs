using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using TeamFlow.Web.Data;
using TeamFlow.Web.Models;

namespace TeamFlow.Web.Areas.Identity.Pages.Account;

[Microsoft.AspNetCore.Authorization.AllowAnonymous]
public class AuthModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly IUserEmailStore<ApplicationUser> _emailStore;
    private readonly ILogger<AuthModel> _logger;
    private readonly IEmailSender _emailSender;
    private readonly SettingsService _settings;

    public AuthModel(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IUserStore<ApplicationUser> userStore,
        ILogger<AuthModel> logger,
        IEmailSender emailSender,
        SettingsService settings)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _logger = logger;
        _emailSender = emailSender;
        _settings = settings;
    }

    [BindProperty]
    public LoginInputModel LoginInput { get; set; } = new();

    [BindProperty]
    public RegisterInputModel RegisterInput { get; set; } = new();

    public string ReturnUrl { get; set; } = "~/";
    public string ActiveTab { get; set; } = "login";
    public bool AllowSelfRegistration { get; set; } = true;

    public class LoginInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterInputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public async Task OnGetAsync(string? tab = null, string? returnUrl = null)
    {
        ActiveTab = string.Equals(tab, "register", StringComparison.OrdinalIgnoreCase) ? "register" : "login";
        ReturnUrl = NormalizeReturnUrl(returnUrl);
        var settings = await _settings.GetAsync();
        AllowSelfRegistration = settings.AllowSelfRegistration;

        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
    }

    public async Task<IActionResult> OnPostLoginAsync(string? returnUrl = null)
    {
        ActiveTab = "login";
        ReturnUrl = NormalizeReturnUrl(returnUrl);
        var settings = await _settings.GetAsync();
        AllowSelfRegistration = settings.AllowSelfRegistration;

        ModelState.Clear();
        if (!TryValidateModel(LoginInput, nameof(LoginInput)))
        {
            return Page();
        }

        var user = await _userManager.FindByEmailAsync(LoginInput.Email);
        if (user != null && !user.IsActive)
        {
            ModelState.AddModelError(string.Empty, "Account is inactive.");
            return Page();
        }

        var result = await _signInManager.PasswordSignInAsync(
            LoginInput.Email,
            LoginInput.Password,
            LoginInput.RememberMe,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            _logger.LogInformation("User logged in.");
            return LocalRedirect(ReturnUrl);
        }

        if (result.RequiresTwoFactor)
        {
            return RedirectToPage("./LoginWith2fa", new { ReturnUrl = ReturnUrl, RememberMe = LoginInput.RememberMe });
        }

        if (result.IsLockedOut)
        {
            _logger.LogWarning("User account locked out.");
            return RedirectToPage("./Lockout");
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return Page();
    }

    public async Task<IActionResult> OnPostRegisterAsync(string? returnUrl = null)
    {
        ActiveTab = "register";
        ReturnUrl = NormalizeReturnUrl(returnUrl);
        var settings = await _settings.GetAsync();
        AllowSelfRegistration = settings.AllowSelfRegistration;

        if (!AllowSelfRegistration)
        {
            ModelState.AddModelError(string.Empty, "Self-registration is disabled. Contact an administrator.");
            return Page();
        }

        ModelState.Clear();
        if (!TryValidateModel(RegisterInput, nameof(RegisterInput)))
        {
            return Page();
        }

        var user = CreateUser();
        user.CurrentStatus = settings.DefaultStatus;
        user.IsActive = true;

        await _userStore.SetUserNameAsync(user, RegisterInput.Email, CancellationToken.None);
        await _emailStore.SetEmailAsync(user, RegisterInput.Email, CancellationToken.None);
        var result = await _userManager.CreateAsync(user, RegisterInput.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation("User created a new account with password.");

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId, code, returnUrl = ReturnUrl },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(RegisterInput.Email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            if (_userManager.Options.SignIn.RequireConfirmedAccount)
            {
                return RedirectToPage("RegisterConfirmation", new { email = RegisterInput.Email, returnUrl = ReturnUrl });
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return LocalRedirect(ReturnUrl);
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return Page();
    }

    private string NormalizeReturnUrl(string? returnUrl)
    {
        var fallback = Url.Content("~/Dashboard");
        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            return fallback;
        }
        if (returnUrl.Contains("/Identity/Account/Logout", StringComparison.OrdinalIgnoreCase))
        {
            return fallback;
        }
        return returnUrl;
    }

    private ApplicationUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<ApplicationUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'.");
        }
    }

    private IUserEmailStore<ApplicationUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<ApplicationUser>)_userStore;
    }
}
