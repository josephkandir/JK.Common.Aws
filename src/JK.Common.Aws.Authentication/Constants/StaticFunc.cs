namespace JK.Common.Aws.Authentication.Constants;

public static class StaticFunc
{
	public static string GetToken(string token)
	{
		if (string.IsNullOrEmpty(token))
			throw new ArgumentNullException(nameof(token), "Empty Token");

		if (!token.StartsWith("Bearer"))
			throw new ArgumentException("Invalid Token");

		if (!token.Contains(' '))
			throw new ArgumentException("Invalid Token");

		token = token.Trim().Split(' ')[1];

		return token;
	}
}
