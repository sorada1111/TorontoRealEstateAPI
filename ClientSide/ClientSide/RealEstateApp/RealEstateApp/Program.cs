using Microsoft.EntityFrameworkCore;
using RealEstateApp.Models;
using RealEstateApp.Service;
using System.Net.Http.Headers;

namespace RealEstateApp
{
    public class Program
    {
        
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(36000); //36000 -> 10hours lifetime
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            //DB
            builder.Services.AddDbContext<UsersContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("Connection2USERSDB")));


            // Define a common method to configure HttpClient
            void ConfigureHttpClient(HttpClient client)
            {
                client.BaseAddress = new Uri("https://34.107.186.229.nip.io/firstproxy/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("apikey", "xo9OQgfbGQBYdhA7cOe0MOMTfZxaX3J6bs0xUzZZfNGSpRaVSySBg4GN0HvaYHfy");
            }

            // Register the HTTP clients using the common configuration method
            builder.Services.AddHttpClient<IPropertyService, PropertyService>(ConfigureHttpClient);
            builder.Services.AddHttpClient<IAddressService, AddressService>(ConfigureHttpClient);
            builder.Services.AddHttpClient<IAgentService, AgentService>(ConfigureHttpClient);
            builder.Services.AddHttpClient<IFeatureService, FeatureService>(ConfigureHttpClient);


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

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}