namespace JK.Common.Aws.Authentication.Contracts;

public class ChangePasswordResponse : BaseResponse
{
	public string? UserId { get; set; }
}