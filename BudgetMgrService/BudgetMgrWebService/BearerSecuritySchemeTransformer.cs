using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace BudgetMgrWebService
{
    internal sealed class BearerSecuritySchemeTransformer(
        IAuthenticationSchemeProvider authenticationSchemeProvider)
        : IOpenApiDocumentTransformer
    {
        public async Task TransformAsync(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            IEnumerable<AuthenticationScheme> schemes =
                await authenticationSchemeProvider.GetAllSchemesAsync();

            if (!schemes.Any(s => s.Name == "Bearer"))
            {
                return;
            }

            document.Components = new OpenApiComponents();
            document.Components.SecuritySchemes = (IDictionary<string, IOpenApiSecurityScheme>?)new Dictionary<string, OpenApiSecurityScheme>();
            document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token.",
            };
        }
    }
}
