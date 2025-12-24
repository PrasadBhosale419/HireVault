using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using HireVault.Web.Models.ViewModels;
using Microsoft.Extensions.Configuration;
using HireVault.Web.Services;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HireVault.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICognitoService _cognitoService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ICognitoService cognitoService,
            ILogger<AccountController> logger)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _cognitoService = cognitoService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = "/")
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _cognitoService.LoginAsync(model);
                
                if (result.Success)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Email),
                        new Claim("AccessToken", result.AccessToken),
                        new Claim("RefreshToken", result.RefreshToken ?? string.Empty),
                        new Claim("IdToken", result.IdToken ?? string.Empty)
                    };
                    
                    // Add user attributes as claims
                    if (result.UserAttributes != null)
                    {
                        foreach (var attr in result.UserAttributes)
                        {
                            claims.Add(new Claim(attr.Key, attr.Value ?? string.Empty));
                        }
                    }

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe,
                            ExpiresUtc = result.ExpiresIn
                        });

                    _logger.LogInformation($"User {model.Email} logged in at {DateTime.UtcNow}");
                    return RedirectToLocal(returnUrl);
                }
                
                // If we get here, there was an error
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error logging in user {model.Email}");
                ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
                return View(model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _cognitoService.SignUpAsync(model);
                
                if (result.Success)
                {
                    // Store email in TempData to use in the confirmation view
                    TempData["Email"] = model.Email;
                    return RedirectToAction("ConfirmEmail");
                }
                
                // If we get here, there was an error
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error signing up user {model.Email}");
                ModelState.AddModelError(string.Empty, "An error occurred during sign up. Please try again.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var accessToken = User.FindFirst("AccessToken")?.Value;
            
            if (!string.IsNullOrEmpty(accessToken))
            {
                try
                {
                    await _cognitoService.SignOutAsync(accessToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error signing out from Cognito");
                    // Continue with local sign out even if Cognito sign out fails
                }
            }
            
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation($"User {User.Identity?.Name} logged out at {DateTime.UtcNow}");
            
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ConfirmEmail()
        {
            var email = TempData["Email"]?.ToString();
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction(nameof(Login));
            }
            
            return View(new ConfirmEmailViewModel { Email = email });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var result = await _cognitoService.ConfirmSignUpAsync(model.Email, model.ConfirmationCode);
            
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Your email has been confirmed. Please log in.";
                return RedirectToAction(nameof(Login));
            }
            
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            try
            {
                await _cognitoService.ForgotPasswordAsync(model.Email);
                // Don't reveal that the user does not exist or is not confirmed
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending password reset email to {model.Email}");
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request. Please try again.");
                return View(model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null, string email = null)
        {
            if (code == null || email == null)
            {
                return BadRequest("A code and email must be supplied for password reset.");
            }
            
            var model = new ResetPasswordViewModel 
            { 
                Code = code, 
                Email = email 
            };
            
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            try
            {
                await _cognitoService.ConfirmForgotPasswordAsync(model.Email, model.Code, model.Password);
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error resetting password for {model.Email}");
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
