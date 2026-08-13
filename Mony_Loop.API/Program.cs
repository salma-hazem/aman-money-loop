using Microsoft.EntityFrameworkCore;
using Mony_Loop.Application.ServicesAbstractions;
using Mony_Loop.Application.Services;
using Mony_Loop.Domain.Interfaces;
using Mony_Loop.Infrastructure.Data;
using Mony_Loop.Infrastructure.Repositories;

namespace Mony_Loop.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<MonyLoopDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IMembershipApplicationRepository, MembershipApplicationRepository>();
            builder.Services.AddScoped<IMembershipApplicationService, MembershipApplicationService>();

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