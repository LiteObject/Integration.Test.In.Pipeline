using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Demo.Product.Api
{
    public static class AuthSetup
    {
        public static void Init(IServiceCollection services)
        {
            _ = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                    {
                        options.TokenValidationParameters =
                        new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                        {
                            ValidateLifetime = true,
                            ValidateAudience = true,
                            ValidAudiences = new[] { "demo-product-api" }
                        };
                    });

            _ = services.AddAuthorization(options => {
                options.AddPolicy("product-write-policy", policy => policy.RequireClaim("scope", "demo-product-api.product.write"));
            });
        }
    }
}
