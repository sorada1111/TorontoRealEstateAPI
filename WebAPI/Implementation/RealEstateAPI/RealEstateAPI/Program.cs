using RealEstateAPI.Connector;
using RealEstateAPI.Services;

namespace RealEstateAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Register AWSConnector service here
            builder.Services.AddSingleton<AWSConnector>();
            builder.Services.AddScoped<IRealEstateRespository, RealEstateRespository>();
            builder.Services.AddScoped<IFileStorageService, FileStorageService>();
            builder.Services.AddScoped<IAddressRepository, AddressRepository>();
            builder.Services.AddScoped<IAgentRepository, AgentRepository>();
            builder.Services.AddScoped<IFeatureRepository, FeatureRepository>();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers().AddNewtonsoftJson();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}