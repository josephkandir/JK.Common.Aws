using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using JK.Common.Aws.Authentication.Contracts;
using JK.Common.Aws.Authentication.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace JK.Common.Aws.Authentication;

public class BaseCognitoManager
{
	protected readonly AwsOptions _awsOptions;
	protected AmazonCognitoIdentityProviderClient _cognito;
	protected readonly HttpContext? _httpContext;

	protected readonly string region;
    protected readonly string clientId;
    protected readonly string userPoolId;
    protected readonly string key;
    protected readonly string signature;

    protected BaseCognitoManager(IOptions<AwsOptions> configuration, IHttpContextAccessor httpContextAccessor)
	{
		_awsOptions = configuration.Value;
		_cognito = new AmazonCognitoIdentityProviderClient(RegionEndpoint.GetBySystemName(_awsOptions.Region));
		_httpContext = httpContextAccessor.HttpContext;

        region = _awsOptions.Region!;
        clientId = _awsOptions.AppClientId!;
        userPoolId = _awsOptions.UserPoolId!;
        key = "";
        signature = "";
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

    protected async Task<AuthenticationResultType> RefreshTokenReponse(RefreshTokenRequest tokenRequest)
    {
        var request = new InitiateAuthRequest
        {
            ClientId = _awsOptions.AppClientId,
            AuthFlow = AuthFlowType.REFRESH_TOKEN_AUTH
        };

        request.AuthParameters.Add("REFRESH_TOKEN", tokenRequest.RefreshToken);

        var initiateAuthResponse = await _cognito.InitiateAuthAsync(request);
        var response = initiateAuthResponse.AuthenticationResult;
        return response;
    }
}
