using Amazon.CognitoIdentityProvider.Model;
using JK.Common.Aws.Authentication.Contracts;

namespace JK.Common.Aws.Authentication;

public interface ICognitoManager
{
	Task<AuthenticationResultType> SignInAsync(LoginRequest request);
	Task<AuthenticationResultType> SignInLiteAsync(LoginRequest request);
	Task<BaseResponse> TryChangePasswordAsync(PasswordChangeRequest request);
    Task<ForgotPasswordResponse> ForgotPasswordAsync(PasswordForgotRequest request);
    Task<ConfirmForgotPasswordResponse> ConfirmForgotPasswordAsync(ConfirmPasswordForgotRequest request);
}
