using Amazon.CognitoIdentityProvider.Model;
using Amazon.CognitoIdentityProvider;
using JK.Common.Aws.Authentication.Contracts;
using JK.Common.Aws.Authentication.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Amazon;

namespace JK.Common.Aws.Authentication;

public class BaseCognitoManager
{
	protected readonly AwsOptions _awsOptions;
	protected readonly AmazonCognitoIdentityProviderClient _cognito;
	protected readonly HttpContext? _httpContext;

	protected BaseCognitoManager(IOptions<AwsOptions> configuration, IHttpContextAccessor httpContextAccessor)
	{
		_awsOptions = configuration.Value;
		_cognito = new AmazonCognitoIdentityProviderClient(RegionEndpoint.GetBySystemName(_awsOptions.Region));
		_httpContext = httpContextAccessor.HttpContext;
	}

	protected async Task<AuthenticationResultType> AuthenticateUserAsync(LoginRequest request, AuthRequestType authRequest)
	{
		AuthenticationResultType response = authRequest == AuthRequestType.Admin ? await AdminAuthResponse(request) : await AuthReponse(request);

		return response;
	}

	private async Task<AuthenticationResultType> AdminAuthResponse(LoginRequest loginRequest)
	{
		var request = new AdminInitiateAuthRequest
		{
			ClientId = _awsOptions.AppClientId,
			UserPoolId = _awsOptions.UserPoolId,
			AuthFlow = AuthFlowType.ADMIN_USER_PASSWORD_AUTH
		};

		request.AuthParameters.Add("USERNAME", loginRequest.Username);
		request.AuthParameters.Add("PASSWORD", loginRequest.Password);

		var initiateAuthResponse = await _cognito.AdminInitiateAuthAsync(request);
		var response = initiateAuthResponse.AuthenticationResult;
		return response;
	}

	private async Task<AuthenticationResultType> AuthReponse(LoginRequest loginRequest)
	{
		var request = new InitiateAuthRequest
		{
			ClientId = _awsOptions.AppClientId,
			AuthFlow = AuthFlowType.USER_PASSWORD_AUTH
		};

		request.AuthParameters.Add("USERNAME", loginRequest.Username);
		request.AuthParameters.Add("PASSWORD", loginRequest.Password);

		var initiateAuthResponse = await _cognito.InitiateAuthAsync(request);
		var response = initiateAuthResponse.AuthenticationResult;
		return response;
	}
}
