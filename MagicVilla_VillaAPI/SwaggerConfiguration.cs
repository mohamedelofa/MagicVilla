using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi.Models;

namespace MagicVilla_VillaAPI
{
	public static class SwaggerConfiguration
	{
		public static IServiceCollection AddSwagerConfiguration(this IServiceCollection service)
		{
			var apiVersionDescriptionProvider = service.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
			service.AddSwaggerGen(options =>
			{
				options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Description =
						"JWT Authorization header using the Bearer scheme. \r\n\r\n " +
						"Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
						"Example: \"Bearer 12345abcdef\"",
					Name = "Authorization",
					In = ParameterLocation.Header,
					Scheme = "Bearer"
				});
				options.AddSecurityRequirement(new OpenApiSecurityRequirement()
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
										{
											Type = ReferenceType.SecurityScheme,
											Id = "Bearer"
										},
							Scheme = "oauth2",
							Name = "Bearer",
							In = ParameterLocation.Header
						},
						new List<string>()
					}
				});
				foreach (var version in apiVersionDescriptionProvider.ApiVersionDescriptions)
				{
					options.SwaggerDoc(version.GroupName, new OpenApiInfo()
					{
						Version = version.ApiVersion.ToString(),
						Description = "Magic_VillaV" + version.ApiVersion.ToString(),
						TermsOfService = new Uri("http://www.facebook.com"),
						Title = "MagicVillaAPI"
					});
				}
			});
			return service;
		}
	}
}

/*
 
 options.SwaggerDoc("v1", new OpenApiInfo()
{
				Version = "v1",
				Description = "Magic_VillaV1",
				TermsOfService = new Uri("http://www.facebook.com"),
				Title = "MagicVillaAPI"
});
options.SwaggerDoc("v2", new OpenApiInfo()
{
				Version = "v2",
				Description = "Magic_VillaV2",
				TermsOfService = new Uri("http://www.facebook.com"),
				Title = "MagicVillaAPI"
});
 
 */