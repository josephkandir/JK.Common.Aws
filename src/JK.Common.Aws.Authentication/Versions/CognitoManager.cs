using Amazon.CognitoIdentityProvider.Model;
using JK.Common.Aws.Authentication.Constants;
using JK.Common.Aws.Authentication.Contracts;
using JK.Common.Aws.Authentication.Enums;
using Microsoft.AspNetCore.Http;

namespace JK.Common.Aws.Authentication.v2;

public class CognitoManager : BaseCognitoManager, ICognitoManager
{
    public CognitoManager(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
    }

    public async Task<ConfirmForgotPasswordResponse> ConfirmForgotPasswordAsync(ConfirmPasswordForgotRequest request)
    {
        var res = new ConfirmForgotPasswordRequest
        {
            Username = request.Username,
            ClientId = clientId,
            ConfirmationCode = request.ConfirmationCode,
            Password = request.Password
        };
        ConfirmForgotPasswordResponse response = await _cognito.ConfirmForgotPasswordAsync(res);
        return response;
    }

    public async Task<ForgotPasswordResponse> ForgotPasswordAsync(PasswordForgotRequest request)
    {
        var res = new ForgotPasswordRequest
        {
            ClientId = clientId,
            Username = request.Username
        };
        ForgotPasswordResponse response = await _cognito.ForgotPasswordAsync(res);
        return response;
    }

    public async Task<AuthenticationResultType> SignInAsync(LoginRequest request)
    {
        AuthenticationResultType response = await AuthenticateUserAsync(request, AuthRequestType.Admin);

        return response;
    }

    public async Task<AuthenticationResultType> SignInLiteAsync(LoginRequest request)
    {
        AuthenticationResultType response = await AuthenticateUserAsync(request, AuthRequestType.NonAdmin);

        return response;
    }

    public async Task<BaseResponse> TryChangePasswordAsync(PasswordChangeRequest request)
    {
        var accessToken = _httpContext!.Request.Headers[HeaderConstant.ACCESS_TOKEN];

        ChangePasswordRequest changePasswordRequest = new ChangePasswordRequest
        {
            AccessToken = accessToken,
            PreviousPassword = request.CurrentPassword,
            ProposedPassword = request.NewPassword
        };

        await _cognito.ChangePasswordAsync(changePasswordRequest);

        return new Contracts.ChangePasswordResponse { UserId = "", Message = "Password Changes", IsSuccess = true };
    }
}

