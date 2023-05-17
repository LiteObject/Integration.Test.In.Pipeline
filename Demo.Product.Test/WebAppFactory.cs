using Demo.Product.Api;
using Demo.Product.Test.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Product.Test
{
    public class WebAppFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");

            // Services can be overridden in a test with a call to ConfigureTestServices on the host builder.
            builder.ConfigureTestServices(services =>
            {
                //services.Configure<AuthenticationSchemeOptions>(options =>
                //{
                //    // options.Validate();
                //});

                services.AddHttpLogging(logging =>
                {
                    logging.LoggingFields = HttpLoggingFields.All;
                    logging.RequestBodyLogLimit = 4096;
                    logging.ResponseBodyLogLimit = 4096;
                });

                services
                .AddAuthentication(TestAuthenticationHandler.AuthenticationSchema)
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(TestAuthenticationHandler.AuthenticationSchema, options => { });
            });
        }
    }
}
