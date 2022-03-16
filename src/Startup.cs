using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CoreCodeCamp.Controllers;
using CoreCodeCamp.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CoreCodeCamp
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext<CampContext>();
      services.AddScoped<ICampRepository, CampRepository>();

      services.AddAutoMapper(Assembly.GetExecutingAssembly()); // --> AutoMapper Extension

      services.AddApiVersioning(); // --> Versioning API

            services.AddApiVersioning(opt =>
            {
                opt.AssumeDefaultVersionWhenUnspecified = true; // Einstellung um ohne Mitgabe von Versionsnummer die API einer Version zuzuordner
                opt.DefaultApiVersion = new ApiVersion(1, 1); // Eingabe von Default Version, API funktioniert nur mit 1,1
                opt.ReportApiVersions = true; // Gibt im Header die Api-Version mit
                opt.ApiVersionReader = new UrlSegmentApiVersionReader(); // Liest Version über URL ab
                //opt.ApiVersionReader = ApiVersionReader.Combine(
                //    new HeaderApiVersionReader("X-Version"), // Liest über den Header "X-Version" den Header --> bisschen zu komplex
                //    new QueryStringApiVersionReader("ver")); // Liest im Query-String "ver" die Version ab
                //    // kann zu Fehlern kommen wenn in beiden zwei verschiedenen Versionen vorkommt

                //opt.Conventions.Controller<TalksController>()
                //    .HasApiVersion(new ApiVersion(1, 0))
                //    .HasApiVersion(new ApiVersion(1, 1))
                //    .Action(c => c.Delete(default(string), default(int)))
                //        .MapToApiVersion(1, 1);
            });

        // schaltet "normales" Endpoint-Routing aus, und aktiviert versioniertes API, d.h. bei jeder Abfrage
        // muss irgendwie die Version mitgegeben werden
        services.AddMvc(opt => opt.EnableEndpointRouting = false)
            .SetCompatibilityVersion(CompatibilityVersion.Latest);

      services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(cfg =>
      {
        cfg.MapControllers();
      });
    }
  }
}
