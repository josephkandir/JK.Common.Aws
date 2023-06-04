namespace JK.Common.Aws.Authentication.Contracts;

public record ConfirmPasswordForgotRequest(string Username, string ConfirmationCode, string Password);