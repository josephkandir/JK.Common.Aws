using JK.Common.Aws.Authentication.Constants;
using JK.Common.Aws.Authentication.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JK.Common.Aws.Authentication;

public static class ServiceCollectionExtensions
{
    public static void AddAWSAuth(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<AwsOptions>(configuration.GetSection("AwsOptions"));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                var _aws = configuration.GetSection(nameof(AwsOptions)).Get<AwsOptions>();
                options.Audience = _aws.AppClientId;
                options.Authority = $"https://cognito-idp.{_aws.Region}.amazonaws.com/{_aws.UserPoolId}";

                options.SaveToken = true;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    RoleClaimType = "cognito:groups"
                };
            });

        services.AddHttpContextAccessor();
        services.AddScoped<ICognitoManager, CognitoManager>();
    }

    /// <summary>
    /// Get AWS and Cognito Credentials through Environment Variables
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void AddAWSAuthVer2(this IServiceCollection services)
    {
        string appClientId = Environment.GetEnvironmentVariable(AwsConstant.COGNITO_APP_CLIENT_ID)!;
        string region = Environment.GetEnvironmentVariable(AwsConstant.COGNITO_REGION)!;
        string userPoolId = Environment.GetEnvironmentVariable(AwsConstant.COGNITO_USER_POOL_ID)!;

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                options.Audience = appClientId;
                options.Authority = $"https://cognito-idp.{region}.amazonaws.com/{userPoolId}";

                options.SaveToken = true;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    RoleClaimType = "cognito:groups"
                };
            });

        services.AddHttpContextAccessor();
        services.AddScoped<ICognitoManager, v2.CognitoManager>();
    }
}
