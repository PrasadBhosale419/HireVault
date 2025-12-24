using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime;
using HireVault.Web.Models;
using HireVault.Web.Models.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace HireVault.Web.Services
{
    public class CognitoService : ICognitoService
    {
        private readonly IAmazonCognitoIdentityProvider _cognitoClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CognitoService> _logger;
        private readonly string _userPoolId;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _region;

        public CognitoService(
            IAmazonCognitoIdentityProvider cognitoClient,
            IConfiguration configuration,
            ILogger<CognitoService> logger)
        {
            _cognitoClient = cognitoClient;
            _configuration = configuration;
            _logger = logger;
            
            _userPoolId = _configuration["AWS:UserPoolId"] ?? throw new ArgumentNullException("AWS:UserPoolId is not configured");
            _clientId = _configuration["AWS:UserPoolClientId"] ?? throw new ArgumentNullException("AWS:UserPoolClientId is not configured");
            _clientSecret = _configuration["AWS:UserPoolClientSecret"];
            _region = _configuration["AWS:Region"] ?? "us-east-1";
        }

        public async Task<AuthResponse> SignUpAsync(SignUpViewModel model)
        {
            try
            {
                var userPool = new CognitoUserPool(_userPoolId, _clientId, _cognitoClient);
                var userAttributes = new Dictionary<string, string>
                {
                    { "email", model.Email },
                    { "given_name", model.FirstName },
                    { "family_name", model.LastName }
                };

                var secretHash = GetSecretHash(model.Email);
                var signUpRequest = new SignUpRequest
                {
                    ClientId = _clientId,
                    SecretHash = secretHash,
                    Username = model.Email,
                    Password = model.Password,
                    UserAttributes = userAttributes
                        .Select(attr => new AttributeType { Name = attr.Key, Value = attr.Value })
                        .ToList()
                };

                var response = await _cognitoClient.SignUpAsync(signUpRequest);
                
                if (response.HttpStatusCode == HttpStatusCode.OK)
                {
                    _logger.LogInformation($"User {model.Email} signed up successfully. UserSub: {response.UserSub}");
                    return AuthResponse.SuccessResponse(null, null, null, DateTime.UtcNow);
                }

                return AuthResponse.ErrorResponse("Failed to sign up user");
            }
            catch (UsernameExistsException)
            {
                _logger.LogWarning($"User with email {model.Email} already exists");
                return AuthResponse.ErrorResponse("A user with this email already exists.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error signing up user {model.Email}");
                return AuthResponse.ErrorResponse($"An error occurred: {ex.Message}");
            }
        }

        public async Task<AuthResponse> ConfirmSignUpAsync(string email, string confirmationCode)
        {
            try
            {
                var confirmRequest = new ConfirmSignUpRequest
                {
                    ClientId = _clientId,
                    SecretHash = GetSecretHash(email),
                    Username = email,
                    ConfirmationCode = confirmationCode
                };

                var response = await _cognitoClient.ConfirmSignUpAsync(confirmRequest);
                
                if (response.HttpStatusCode == HttpStatusCode.OK)
                {
                    _logger.LogInformation($"User {email} confirmed successfully");
                    return AuthResponse.SuccessResponse(null, null, null, DateTime.UtcNow);
                }

                return AuthResponse.ErrorResponse("Failed to confirm user");
            }
            catch (UserNotFoundException)
            {
                _logger.LogWarning($"User {email} not found during confirmation");
                return AuthResponse.ErrorResponse("User not found.");
            }
            catch (CodeMismatchException)
            {
                _logger.LogWarning($"Invalid confirmation code for user {email}");
                return AuthResponse.ErrorResponse("Invalid verification code provided.");
            }
            catch (ExpiredCodeException)
            {
                _logger.LogWarning($"Expired confirmation code for user {email}");
                return AuthResponse.ErrorResponse("The verification code has expired. Please request a new one.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error confirming user {email}");
                return AuthResponse.ErrorResponse($"An error occurred: {ex.Message}");
            }
        }

        public async Task<AuthResponse> LoginAsync(LoginViewModel model)
        {
            try
            {
                var userPool = new CognitoUserPool(_userPoolId, _clientId, _cognitoClient);
                var user = new CognitoUser(model.Email, _clientId, userPool, _cognitoClient, _clientSecret);
                
                var authRequest = new InitiateSrpAuthRequest()
                {
                    Password = model.Password
                };

                var authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);
                
                if (authResponse.AuthenticationResult != null)
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var token = tokenHandler.ReadJwtToken(authResponse.AuthenticationResult.IdToken);
                    var expiresIn = token.ValidTo;
                    
                    var userAttributes = new Dictionary<string, string>();
                    foreach (var claim in token.Claims)
                    {
                        if (claim.Type.StartsWith("custom:"))
                        {
                            userAttributes[claim.Type.Substring(7)] = claim.Value;
                        }
                    }
                    
                    _logger.LogInformation($"User {model.Email} logged in successfully");
                    return AuthResponse.SuccessResponse(
                        authResponse.AuthenticationResult.AccessToken,
                        authResponse.AuthenticationResult.IdToken,
                        authResponse.AuthenticationResult.RefreshToken,
                        expiresIn,
                        userAttributes);
                }
                
                return AuthResponse.ErrorResponse("Authentication failed");
            }
            catch (UserNotConfirmedException)
            {
                _logger.LogWarning($"User {model.Email} not confirmed");
                return AuthResponse.ErrorResponse("User is not confirmed. Please check your email for the verification code.");
            }
            catch (NotAuthorizedException)
            {
                _logger.LogWarning($"Invalid credentials for user {model.Email}");
                return AuthResponse.ErrorResponse("Incorrect username or password.");
            }
            catch (UserNotFoundException)
            {
                _logger.LogWarning($"User {model.Email} not found");
                return AuthResponse.ErrorResponse("Incorrect username or password.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error logging in user {model.Email}");
                return AuthResponse.ErrorResponse($"An error occurred during login: {ex.Message}");
            }
        }

        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var user = new CognitoUser("", _clientId, null, _cognitoClient, _clientSecret);
                user.SessionTokens = new CognitoUserSession(null, null, refreshToken, DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
                
                var authResponse = await user.StartWithRefreshTokenAuthAsync(new InitiateRefreshTokenAuthRequest
                {
                    AuthFlowType = AuthFlowType.REFRESH_TOKEN_AUTH
                }).ConfigureAwait(false);

                if (authResponse.AuthenticationResult != null)
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var token = tokenHandler.ReadJwtToken(authResponse.AuthenticationResult.IdToken);
                    var expiresIn = token.ValidTo;
                    
                    _logger.LogInformation("Token refreshed successfully");
                    return AuthResponse.SuccessResponse(
                        authResponse.AuthenticationResult.AccessToken,
                        authResponse.AuthenticationResult.IdToken,
                        authResponse.AuthenticationResult.RefreshToken,
                        expiresIn);
                }
                
                return AuthResponse.ErrorResponse("Failed to refresh token");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return AuthResponse.ErrorResponse($"An error occurred while refreshing token: {ex.Message}");
            }
        }

        public async Task ForgotPasswordAsync(string email)
        {
            try
            {
                var request = new ForgotPasswordRequest
                {
                    ClientId = _clientId,
                    SecretHash = GetSecretHash(email),
                    Username = email
                };

                await _cognitoClient.ForgotPasswordAsync(request);
                _logger.LogInformation($"Password reset code sent to {email}");
            }
            catch (UserNotFoundException)
            {
                _logger.LogWarning($"User {email} not found during forgot password request");
                // Don't reveal that the user doesn't exist
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error initiating forgot password for {email}");
                throw;
            }
        }

        public async Task ConfirmForgotPasswordAsync(string email, string confirmationCode, string newPassword)
        {
            try
            {
                var request = new ConfirmForgotPasswordRequest
                {
                    ClientId = _clientId,
                    SecretHash = GetSecretHash(email),
                    Username = email,
                    ConfirmationCode = confirmationCode,
                    Password = newPassword
                };

                await _cognitoClient.ConfirmForgotPasswordAsync(request);
                _logger.LogInformation($"Password reset confirmed for {email}");
            }
            catch (CodeMismatchException)
            {
                _logger.LogWarning($"Invalid confirmation code for user {email}");
                throw new Exception("Invalid verification code provided.");
            }
            catch (ExpiredCodeException)
            {
                _logger.LogWarning($"Expired confirmation code for user {email}");
                throw new Exception("The verification code has expired. Please request a new one.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error confirming password reset for {email}");
                throw new Exception($"An error occurred while resetting your password: {ex.Message}");
            }
        }

        public async Task SignOutAsync(string accessToken)
        {
            try
            {
                await _cognitoClient.GlobalSignOutAsync(new GlobalSignOutRequest
                {
                    AccessToken = accessToken
                });
                
                _logger.LogInformation("User signed out successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error signing out user");
                throw;
            }
        }

        private string GetSecretHash(string username)
        {
            if (string.IsNullOrEmpty(_clientSecret))
                return null;

            var message = username + _clientId;
            var encoding = new System.Text.UTF8Encoding();
            var keyBytes = encoding.GetBytes(_clientSecret);
            var messageBytes = encoding.GetBytes(message);
            using (var hmac = new System.Security.Cryptography.HMACSHA256(keyBytes))
            {
                var hash = hmac.ComputeHash(messageBytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
