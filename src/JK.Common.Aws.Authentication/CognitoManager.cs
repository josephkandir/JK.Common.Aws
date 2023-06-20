using Amazon.CognitoIdentityProvider.Model;
using JK.Common.Aws.Authentication.Constants;
using JK.Common.Aws.Authentication.Contracts;
using JK.Common.Aws.Authentication.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using JkCommonModel = JK.Common.Aws.Authentication.Contracts;

namespace JK.Common.Aws.Authentication;

public class CognitoManager : BaseCognitoManager, ICognitoManager
{
	public CognitoManager(IOptions<AwsOptions> configuration, IHttpContextAccessor httpContextAccessor) : base(configuration, httpContextAccessor) { }

    public async Task<ConfirmForgotPasswordResponse> ConfirmForgotPasswordAsync(ConfirmPasswordForgotRequest request)
    {
		var res = new ConfirmForgotPasswordRequest
		{
			Username = request.Username,
			ClientId = _awsOptions.AppClientId,
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
            ClientId = _awsOptions.AppClientId,
            Username = request.Username
        };
        ForgotPasswordResponse response = await _cognito.ForgotPasswordAsync(res);
		return response;
    }

    public async Task<List<UserType>> ListUsersAsync()
    {
        var request = new ListUsersRequest
        {
            UserPoolId = userPoolId
        };

        var users = new List<UserType>();

        var usersPaginator = _cognito.Paginators.ListUsers(request);
        await foreach (var response in usersPaginator.Responses)
        {
            users.AddRange(response.Users);
        }

        return users;
    }

    public async Task<AuthenticationResultType> RefreshTokenAsync(RefreshTokenRequest request)
    {
        AuthenticationResultType response = await RefreshTokenReponse(request);

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

		ChangePasswordRequest changePasswordRequest = new ChangePasswordRequest
		{
			AccessToken = accessToken,
			PreviousPassword = request.CurrentPassword,
			ProposedPassword = request.NewPassword
		};

		await _cognito.ChangePasswordAsync(changePasswordRequest);

		return new JkCommonModel.ChangePasswordResponse { UserId = "", Message = "Password Changes", IsSuccess = true };
	}
}
