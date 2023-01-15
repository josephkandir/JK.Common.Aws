using JK.Common.Aws.Authentication.Enums;

namespace JK.Common.Aws.Authentication.Contracts;

public class BaseResponse
{
	public bool IsSuccess { get; set; }
	public string? Message { get; set; }
	public CognitoStatusCodes Status { get; set; }
}