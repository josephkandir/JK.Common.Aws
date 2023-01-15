using Amazon.CognitoIdentityProvider.Model;
using JK.Common.Aws.Authentication.Contracts;
using ChangePasswordRequest = JK.Common.Aws.Authentication.Contracts.ChangePasswordRequest;

namespace JK.Common.Aws.Authentication;

public interface ICognitoManager
{
	Task<AuthenticationResultType> SignInAsync(LoginRequest request);
	Task<AuthenticationResultType> SignInLiteAsync(LoginRequest request);
	Task<BaseResponse> TryChangePasswordAsync(ChangePasswordRequest request);
}
