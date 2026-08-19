
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MonyLoop.Application.Profiles.AgreementPayment;
using MonyLoop.Application.Profiles.OnboardingMemberLedger;
using MonyLoop.Application.Services;
using MonyLoop.Application.Services.AgreementPayment;
using MonyLoop.Application.Services.OnboardingMemberLedger;
using MonyLoop.Application.Services.UserAuth;
using MonyLoop.Application.ServicesAbstractions;
using MonyLoop.Application.ServicesAbstractions.AgreementPayment;
using MonyLoop.Application.ServicesAbstractions.OnboardingMemberLedger;
using MonyLoop.Application.ServicesAbstractions.UserAuth;
using MonyLoop.Domain.Entities.UserAuth;
using MonyLoop.Domain.Interfaces;
using MonyLoop.Domain.Interfaces.AgreementPayment;
using MonyLoop.Infrastructure;
using MonyLoop.Infrastructure.Data;
using MonyLoop.Infrastructure.Repositories;
using MonyLoop.Infrastructure.Repositories.AgreementPayment;
using MonyLoop.Infrastructure.Repositories.CircleRequestManagement;
using MonyLoop.Infrastructure.Repositories.OnboardingMemberLedger;
using MonyLoop.Infrastructure.Services.Email;
using QuestPDF.Infrastructure;

namespace MonyLoop.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            QuestPDF.Settings.License = LicenseType.Community;

            // Controllers
            builder.Services.AddControllers();

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // AutoMapper
            builder.Services.AddAutoMapper(typeof(AgreementPaymentProfile).Assembly);
            builder.Services.AddAutoMapper(typeof(OnboardingMemberLedgerProfile).Assembly);

            // Module 5 - Services
            builder.Services.AddScoped<
                IMembershipAgreementService,
                MembershipAgreementService>();

            builder.Services.AddScoped<
                IPaymentTransactionService,
                PaymentTransactionService>();

            builder.Services.AddScoped<
                IPaymentReceiptPdfService,
                PaymentReceiptPdfService>();

            // Module 6 
            builder.Services.AddScoped<IDocumentService, DocumentService>();
            builder.Services.AddScoped<IDocumentRequirementService, DocumentRequirementService>();
            builder.Services.AddScoped<IOnboardingCaseService, OnboardingCaseService>();
            builder.Services.AddScoped<IMemberLedgerService, MemberLedgerService>();

            //Module 1 
            builder.Services.AddScoped<IOTPService, OTPService>();
            builder.Services.AddScoped<IEmailTemplateRenderer, RazorLightEmailRenderer>();
            builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();



            // Module 5 - Repositories
            builder.Services.AddScoped<IMembershipAgreementRepository, MembershipAgreementRepository>();
            builder.Services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();

            // Module 3 - Services / Repositories
            builder.Services.AddScoped<IMembershipApplicationRepository, MembershipApplicationRepository>();
            builder.Services.AddScoped<IMembershipApplicationService, MembershipApplicationService>();

            // Database
            builder.Services.AddDbContext<MonyLoopDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            //Hangfire
            builder.Services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddHangfireServer();


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
            builder.Services.AddOnboardingMemberLedgerRepositories();


            var app = builder.Build();

            // HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseHangfireDashboard("/hangfire");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
