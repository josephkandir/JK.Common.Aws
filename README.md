### To Use AWS Sdk Environment Variable To Be Added
Can be added in Asp.Net Core Project or System Environment Variable
AWS_ACCESS_KEY_ID=your_access_key_id
AWS_SECRET_KEY=your_access_secret_key

### C# Class in AWSAuth Project
```csharp
public class AWSOptions
{
    public string Region { get; set; }
    public string UserPoolId { get; set; }
    public string AppClientId { get; set; }
    public string AppClientSecret { get; set; }
}
```

### Add properties in AppSeetings.json
```json
"AWSOptions": {
  "Region": "ap-south-1",
  "UserPoolId": "ap-south-1_123456789",
  "AppClientId": "12345678901234567890123456",
  "AppClientSecret": ""
}
```

### Add Dependency Injection to main API Project
```charp
using AWSAuth;

// To be added before .AddController() or .AddControllerViewss()
builder.Services.AddAWSAuth(builder.Configuration);
```

### Packages are installed
```csproj
<ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="6.0.0" />
    <PackageReference Include="AWSSDK.CognitoIdentityProvider" Version="3.7.5.7" />
    <PackageReference Include="AWSSDK.Core" Version="3.7.12.23" />
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="6.0.8" />
</ItemGroup>
```

