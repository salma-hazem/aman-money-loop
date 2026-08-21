
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
using MonyLoop.Application.Services.Verification;
using MonyLoop.Application.ServicesAbstractions.Verification;
using MonyLoop.Domain.Interfaces.Verification;
using MonyLoop.Infrastructure.Repositories.Verification;
using MonyLoop.Infrastructure.Repositories;
using MonyLoop.Infrastructure.Repositories.AgreementPayment;
using MonyLoop.Infrastructure.Repositories.CircleRequestManagement;
using MonyLoop.Infrastructure.Repositories.OnboardingMemberLedger;
using MonyLoop.Infrastructure.Services.Email;
using QuestPDF.Infrastructure;
using MonyLoop.Application.Settings;
using MonyLoop.Application.Services.CircleRequestManagement;
using MonyLoop.Application.ServicesAbstractions.CircleRequestManagement;
using MonyLoop.API.Swagger;

namespace MonyLoop.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            QuestPDF.Settings.License = LicenseType.Community;

            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddScoped<IEmailService, EmailService>();

            // Controllers
            builder.Services.AddControllers();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AngularDev", policy =>
                {
                    policy
                        .WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
                options.SchemaFilter<CircleRequestEnumSchemaFilter>());

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

            // Module 4 - Verification Management
            builder.Services.AddScoped<IVerificationRoundRepository, VerificationRoundRepository>();
            builder.Services.AddScoped<IVerificationScheduleRepository, VerificationScheduleRepository>();
            builder.Services.AddScoped<IVerificationCriterionRepository, VerificationCriterionRepository>();
            builder.Services.AddScoped<IVerificationCriterionRatingRepository, VerificationCriterionRatingRepository>();
            builder.Services.AddScoped<IVerificationChecklistSubmissionRepository, VerificationChecklistSubmissionRepository>();

            builder.Services.AddScoped<IVerificationService, VerificationService>();

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

            // Module 2 - Circle Request Management
            builder.Services.AddSingleton(TimeProvider.System);
            builder.Services.AddScoped<ICircleRequestService, CircleRequestService>();
            builder.Services.AddScoped<ICircleRequestReviewService, CircleRequestReviewService>();
            builder.Services.AddScoped<CircleRegistryService>();
            builder.Services.AddScoped<ICircleRegistryService>(provider =>
                provider.GetRequiredService<CircleRegistryService>());
            builder.Services.AddScoped<ISlotAssignmentService>(provider =>
                provider.GetRequiredService<CircleRegistryService>());
            builder.Services.AddScoped<IListingAvailabilityService,
                ListingAvailabilityService>();

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

            app.UseCors("AngularDev");
            app.UseHttpsRedirection();

            app.UseHangfireDashboard("/hangfire");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
