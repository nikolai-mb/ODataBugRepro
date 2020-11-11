using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;

namespace ODataBugRepro
{
    public class Startup
    {
        private static readonly bool _useEndpointRouting = false;

        public void ConfigureServices(IServiceCollection services)
        {
            // Enable Azure AD authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
            {
                o.Authority = "https://fakeauthority.com";
            });

            if (!_useEndpointRouting)
                services.AddMvc(options => options.EnableEndpointRouting = false);

            services.AddControllers();
            services.AddApiVersioning(options => options.ReportApiVersions = true);
            services.AddOData().EnableApiVersioning();
            services.AddODataApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddSwaggerGen(options =>
            {
                options.OperationFilter<SwaggerDefaultValues>();
            });
        }

        public void Configure(
            IApplicationBuilder app,
            VersionedODataModelBuilder modelBuilder,
            IApiVersionDescriptionProvider provider)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                if (_useEndpointRouting)
                {
                    endpoints.Select().Expand().Filter().OrderBy().MaxTop(null).Count();
                    endpoints.MapVersionedODataRoute("odata", "api/v{version:apiVersion}", modelBuilder.GetEdmModels());
                }
            });

            if (!_useEndpointRouting)
            {
                app.UseMvc(builder =>
                {
                    builder.Select().Expand().Filter().OrderBy().MaxTop(null).Count();
                    builder.MapVersionedODataRoute("odata", "api/v{version:apiVersion}", modelBuilder.GetEdmModels());
                });
            }

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });
        }
    }
}
