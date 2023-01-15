namespace JK.Common.Aws.Authentication.Contracts;

public record ChangePasswordRequest(string CurrentPassword, string EmailAddress, string NewPassword);