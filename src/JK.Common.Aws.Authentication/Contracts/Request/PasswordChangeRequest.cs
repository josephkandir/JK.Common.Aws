namespace JK.Common.Aws.Authentication.Contracts;

public record PasswordChangeRequest(string CurrentPassword, string EmailAddress, string NewPassword);