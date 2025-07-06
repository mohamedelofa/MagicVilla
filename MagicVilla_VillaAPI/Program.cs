using Asp.Versioning;
using MagicVilla_VillaAPI.Helpers;
using MagicVilla_VillaAPI.Services.Implementation;
using MagicVilla_VillaAPI.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MagicVilla_VillaAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddDbContext<AppDbContext>(
				options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
				?? throw new InvalidOperationException("Error in connection string"))
				);

			builder.Services.AddStackExchangeRedisCache(options =>
			{
				var redissettings = builder.Configuration.GetSection("Redis").Get<RedisSettings>() ?? throw new InvalidOperationException("Error in Redis settings");
				options.ConfigurationOptions = new()
				{
					EndPoints = { redissettings.Endpoint ?? throw new InvalidOperationException("Error in Redis connection string") },
					Password = redissettings.Password,
					Ssl = false,
					AbortOnConnectFail = false,
				};
				options.InstanceName = redissettings.InstanceName;

			});
			builder.Services.AddScoped<IVillaRepository, VillaRepository>();
			builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();
			builder.Services.AddScoped<IUserRepository, UserRepository>();
			builder.Services.AddScoped<ICacheService, CacheService>();
			builder.Services.AddScoped<ApiResponse>();
			builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();

			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

			}).AddJwtBearer(options =>
							  {
								  options.SaveToken = true;
								  options.RequireHttpsMetadata = false;
								  options.TokenValidationParameters = new TokenValidationParameters()
								  {
									  ValidateIssuerSigningKey = true,
									  IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("ApiSettings:Secret"))),
									  ValidateIssuer = false,
									  ValidateAudience = false,
									  ClockSkew = TimeSpan.Zero
								  };

							  });

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();

			// Versioning 
			builder.Services.AddApiVersioning(options =>
			{
				options.AssumeDefaultVersionWhenUnspecified = true;
				options.DefaultApiVersion = ApiVersion.Default;//new ApiVersion(1);
				options.ReportApiVersions = true;
				options.ApiVersionReader = ApiVersionReader.Combine(
					 //new QueryStringApiVersionReader("version"),
					 new UrlSegmentApiVersionReader()
					);
			}).AddMvc()
			.AddApiExplorer(options =>
			{
				options.GroupNameFormat = "'v'VVV";
				options.SubstituteApiVersionInUrl = true;
			});
			builder.Services.AddSwagerConfiguration();

			builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

			var app = builder.Build();
			app.UseSwagger();
			// Configure the HTTP request pipeline.

			if (app.Environment.IsDevelopment())
			{
				app.UseSwaggerUI(options =>
				{
					options.SwaggerEndpoint("/swagger/v2/swagger.json", "Magic_VillaV2");
					options.SwaggerEndpoint("/swagger/v1/swagger.json", "Magic_VillaV1");

				});
			}
			else
			{
				app.UseSwaggerUI(options =>
				{
					options.SwaggerEndpoint("/swagger/v1/swagger.json", "Magic_VillaV1");
					options.SwaggerEndpoint("/swagger/v2/swagger.json", "Magic_VillaV2");
					options.RoutePrefix = string.Empty;
				});
			}
			app.UseStaticFiles();
			app.UseHttpsRedirection();
			app.UseAuthentication();
			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
