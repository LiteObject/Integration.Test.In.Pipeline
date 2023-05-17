using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Demo.Product.Test.Auth
{
    public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string AuthenticationSchema = "TestSchema";

        public TestAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            List<Claim> claims = new()
            {
                new Claim("scope", "demo-product-api.product.write")
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, AuthenticationSchema);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            AuthenticationTicket authenticationTicket = new(principal, AuthenticationSchema);

            AuthenticateResult authenticateResult = AuthenticateResult.Success(authenticationTicket);

            return await Task.FromResult(authenticateResult);
        }
    }
}
