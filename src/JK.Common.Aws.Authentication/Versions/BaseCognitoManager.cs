using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;
using JK.Common.Aws.Authentication.Constants;
using JK.Common.Aws.Authentication.Contracts;
using JK.Common.Aws.Authentication.Enums;
using Microsoft.AspNetCore.Http;

namespace JK.Common.Aws.Authentication.v2;

public class BaseCognitoManager
{
    protected readonly string region = Environment.GetEnvironmentVariable(AwsConstant.COGNITO_REGION)!;
    protected readonly string clientId = Environment.GetEnvironmentVariable(AwsConstant.COGNITO_APP_CLIENT_ID)!;
    protected readonly string userPoolId = Environment.GetEnvironmentVariable(AwsConstant.COGNITO_USER_POOL_ID)!;
    protected readonly string key = Environment.GetEnvironmentVariable(AwsConstant.AWS_ACCESS_KEY)!;
    protected readonly string signature = Environment.GetEnvironmentVariable(AwsConstant.AWS_SIGNATURE)!;

    protected AmazonCognitoIdentityProviderClient _cognito;
    protected readonly HttpContext? _httpContext;

    protected BaseCognitoManager(IHttpContextAccessor httpContextAccessor)
    {
        var credentials = new BasicAWSCredentials(key, signature);
        _cognito = new AmazonCognitoIdentityProviderClient(credentials, RegionEndpoint.GetBySystemName(region));
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
            ClientId = clientId,
            UserPoolId = userPoolId,
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
            ClientId = clientId,
            AuthFlow = AuthFlowType.USER_PASSWORD_AUTH
        };

        request.AuthParameters.Add("USERNAME", loginRequest.Username);
        request.AuthParameters.Add("PASSWORD", loginRequest.Password);

        var initiateAuthResponse = await _cognito.InitiateAuthAsync(request);
        var response = initiateAuthResponse.AuthenticationResult;
        return response;
    }
}

