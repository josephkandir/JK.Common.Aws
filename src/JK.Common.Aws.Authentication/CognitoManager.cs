using Amazon.CognitoIdentityProvider.Model;
using JK.Common.Aws.Authentication.Constants;
using JK.Common.Aws.Authentication.Contracts;
using JK.Common.Aws.Authentication.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using JkCommonModel = JK.Common.Aws.Authentication.Contracts;
using AwsModel = Amazon.CognitoIdentityProvider.Model;

namespace JK.Common.Aws.Authentication;

public class CognitoManager : BaseCognitoManager, ICognitoManager
{
	public CognitoManager(IOptions<AwsOptions> configuration, IHttpContextAccessor httpContextAccessor) : base(configuration, httpContextAccessor) { }

    public async Task<ConfirmForgotPasswordResponse> ConfirmForgotPasswordAsync(ConfirmPasswordForgotRequest request)
    {
		string clientId = Environment.GetEnvironmentVariable(AwsConstant.COGNITO_APP_CLIENT_ID)!.Trim();

		var res = new ConfirmForgotPasswordRequest
		{
			Username = request.Username,
			ClientId = string.IsNullOrEmpty(clientId) ? _awsOptions.AppClientId : clientId,
			ConfirmationCode = request.ConfirmationCode,
			Password = request.Password
		};
        ConfirmForgotPasswordResponse response = await _cognito.ConfirmForgotPasswordAsync(res);
		return response;
    }

    public async Task<ForgotPasswordResponse> ForgotPasswordAsync(PasswordForgotRequest request)
    {
        string clientId = Environment.GetEnvironmentVariable(AwsConstant.COGNITO_APP_CLIENT_ID)!.Trim();

        var res = new ForgotPasswordRequest
        {
            ClientId = string.IsNullOrEmpty(clientId) ? _awsOptions.AppClientId : clientId,
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

	public async Task<BaseResponse> TryChangePasswordAsync(JkCommonModel.PasswordChangeRequest request)
	{
		var accessToken = _httpContext!.Request.Headers[HeaderConstant.ACCESS_TOKEN];

		AwsModel.ChangePasswordRequest changePasswordRequest = new AwsModel.ChangePasswordRequest
		{
			AccessToken = accessToken,
			PreviousPassword = request.CurrentPassword,
			ProposedPassword = request.NewPassword
		};

		await _cognito.ChangePasswordAsync(changePasswordRequest);

		return new JkCommonModel.ChangePasswordResponse { UserId = "", Message = "Password Changes", IsSuccess = true };
	}
}
