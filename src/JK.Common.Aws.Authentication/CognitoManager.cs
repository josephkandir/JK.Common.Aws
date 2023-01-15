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

	public async Task<BaseResponse> TryChangePasswordAsync(JkCommonModel.ChangePasswordRequest request)
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
