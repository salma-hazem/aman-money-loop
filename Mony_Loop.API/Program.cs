
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mony_Loop.Infrastructure.Data;
using MonyLoop.Domain.Entities.UserAuth;
using Mony_Loop.Application.Profiles.AgreementPayment;
using Mony_Loop.Application.Services.AgreementPayment;
using Mony_Loop.Application.ServicesAbstractions.AgreementPayment;

namespace MonyLoop.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddAutoMapper(typeof(AgreementPaymentProfile).Assembly);
            builder.Services.AddScoped<
                IMembershipAgreementService,
                MembershipAgreementService>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.AddDbContext<MonyLoopDbContext>
                (options =>
                {
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

                });

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<MonyLoopDbContext>()
                .AddDefaultTokenProviders();


            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
