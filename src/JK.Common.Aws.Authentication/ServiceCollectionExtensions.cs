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
}
