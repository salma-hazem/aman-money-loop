
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MonyLoop.Application.Profiles.AgreementPayment;
using MonyLoop.Application.Services.AgreementPayment;
using MonyLoop.Application.ServicesAbstractions.AgreementPayment;
using MonyLoop.Domain.Interfaces.AgreementPayment;
using MonyLoop.Infrastructure.Data;
using MonyLoop.Infrastructure.Repositories;
using MonyLoop.Infrastructure.Repositories.AgreementPayment;
using MonyLoop.Infrastructure.Repositories.OnboardingMemberLedger;
using MonyLoop.Domain.Entities.UserAuth;
using MonyLoop.Domain.Interfaces;
using MonyLoop.Domain.Interfaces.OnboardingMemberLedger;

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

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IOnboardingCaseRepository, OnboardingCaseRepository>();
            builder.Services.AddScoped<IDocumentRequirementRepository, DocumentRequirementRepository>();
            builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
            builder.Services.AddScoped<IMemberLedgerRepository, MemberLedgerRepository>();





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