
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mony_Loop.Application.Profiles.AgreementPayment;
using Mony_Loop.Application.Services.AgreementPayment;
using Mony_Loop.Application.ServicesAbstractions.AgreementPayment;
using Mony_Loop.Domain.Interfaces.AgreementPayment;
using Mony_Loop.Infrastructure.Data;
using Mony_Loop.Infrastructure.Repositories.AgreementPayment;
using Mony_Loop.Infrastructure.Repositories.CircleRequestManagement;

using Mony_Loop.Infrastructure.Repositories;
using Mony_Loop.Infrastructure.Repositories.AgreementPayment;
using MonyLoop.Domain.Entities.UserAuth;
using Mony_Loop.Domain.Interfaces;

namespace MonyLoop.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Controllers
            builder.Services.AddControllers();

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // AutoMapper
            builder.Services.AddAutoMapper(
                typeof(AgreementPaymentProfile).Assembly);

            // Module 5 - Services
            builder.Services.AddScoped<
                IMembershipAgreementService,
                MembershipAgreementService>();

            // Module 5 - Repositories
            builder.Services.AddScoped<
                IMembershipAgreementRepository,
                MembershipAgreementRepository>();

            // Database
            builder.Services.AddDbContext<MonyLoopDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // Identity
            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan =
                    TimeSpan.FromMinutes(15);

                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<MonyLoopDbContext>()
            .AddDefaultTokenProviders();

            // Unit of Work and modules Repositories
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddCircleRequestManagementRepositories();

            var app = builder.Build();

            // HTTP request pipeline
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
