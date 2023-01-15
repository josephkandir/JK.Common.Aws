namespace JK.Common.Aws.Authentication.Contracts;

public record AwsToken(
	string? IdToken,
	string? AccessToken,
	string? RefreshToken,
	int ExpiresIn);