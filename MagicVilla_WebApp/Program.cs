using MagicVilla_WebApp.Filters;
using MagicVilla_WebApp.Services;
using MagicVilla_WebApp.Services.IServices;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MagicVilla_WebApp
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews(x => x.Filters.Add(new TokenExceptionRedirection()));
			builder.Services.AddHttpClient<IConsumeService, ConsumeService>();
			//builder.Services.AddHttpClient<IVillaNumberService, VillaNumberService>();
			builder.Services.AddScoped<IVillaService, VillaService>();
			builder.Services.AddScoped<ITokenService, TokenService>();
			builder.Services.AddScoped<IVillaNumberService, VillaNumberService>();
			builder.Services.AddScoped<IAuthService, AuthenticationService>();
			builder.Services.AddScoped<IConsumeService, ConsumeService>();
			builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
			builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(options =>
				{
					options.Cookie.HttpOnly = true;
					options.ExpireTimeSpan = TimeSpan.FromDays(30);
					options.LoginPath = "/Authentication/LogIn";
					options.AccessDeniedPath = "/Authentication/AccessDenied";
				});

			builder.Services.AddDistributedMemoryCache();
			builder.Services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromMinutes(60);
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});
			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseSession();
			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
		}
	}
}
