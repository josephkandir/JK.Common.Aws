namespace JK.Common.Aws.Authentication.Contracts;

public class AwsOptions
{
	public string? Region { get; set; }
	public string? UserPoolId { get; set; }
	public string? AppClientId { get; set; }
	public string? AppClientSecret { get; set; }
}
