using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MonyLoop.API.Extentions;
using MonyLoop.API.Factories;
using MonyLoop.API.Middlewares;
using MonyLoop.API.Swagger;
using MonyLoop.Application.Profiles.AgreementPayment;
using MonyLoop.Application.Profiles.OnboardingMemberLedger;
using MonyLoop.Application.Services;
using MonyLoop.Application.Services.AgreementPayment;
using MonyLoop.Application.Services.CircleRequestManagement;
using MonyLoop.Application.Services.OnboardingMemberLedger;
using MonyLoop.Application.Services.UserAuth;
using MonyLoop.Application.ServicesAbstractions;
using MonyLoop.Application.ServicesAbstractions.AgreementPayment;
using MonyLoop.Application.ServicesAbstractions.CircleRequestManagement;
using MonyLoop.Application.ServicesAbstractions.OnboardingMemberLedger;
using MonyLoop.Application.ServicesAbstractions.UserAuth;
using MonyLoop.Domain.Entities.UserAuth;
using MonyLoop.Domain.Interfaces;
using MonyLoop.Domain.Interfaces.AgreementPayment;
using MonyLoop.Domain.Interfaces.OnboardingMemberLedger;
using MonyLoop.Domain.Interfaces.UserAuth;
using MonyLoop.Infrastructure;
using MonyLoop.Infrastructure.Data;
using MonyLoop.Domain.Interfaces.Verification;
using MonyLoop.Infrastructure.Repositories.Verification;
using MonyLoop.Domain.Interfaces.Verification;
using MonyLoop.Infrastructure.Repositories.Verification;
using MonyLoop.Infrastructure.DataSeeding;
using MonyLoop.Application.Services.Verification;
using MonyLoop.Application.ServicesAbstractions.Verification;
using MonyLoop.Domain.Interfaces.Verification;
using MonyLoop.Infrastructure.Repositories.Verification;
using MonyLoop.Infrastructure.Repositories;
using MonyLoop.Infrastructure.Repositories.AgreementPayment;
using MonyLoop.Infrastructure.Repositories.CircleRequestManagement;
using MonyLoop.Infrastructure.Repositories.OnboardingMemberLedger;
using MonyLoop.Infrastructure.Repositories.UserAuth;
using MonyLoop.Infrastructure.Services.Email;
using MonyLoop.Infrastructure.Services.UserAuth;
using QuestPDF.Infrastructure;
using StackExchange.Redis;
using System.Text;

namespace MonyLoop.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            QuestPDF.Settings.License = LicenseType.Community;

            // ===== Controllers & API Behavior =====
            builder.Services.AddControllers();
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = ApiResponseFactory.GenerateApiValidationResponse;
            });

            // ===== CORS =====
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

            // ===== Swagger =====
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
                options.SchemaFilter<CircleRequestEnumSchemaFilter>());

            // ===== AutoMapper =====
            builder.Services.AddAutoMapper(typeof(AgreementPaymentProfile).Assembly);
            builder.Services.AddAutoMapper(typeof(OnboardingMemberLedgerProfile).Assembly);

            // ===== Database =====
            builder.Services.AddDbContext<MonyLoopDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // ===== Unit of Work =====
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

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
            // ===== Module 1 - Identity / Auth =====
            builder.Services.AddScoped<IOTPTokenRepository, OTPTokenRepository>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            builder.Services.AddScoped<IOTPService, OTPService>();
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IEmailTemplateRenderer, RazorLightEmailRenderer>();
            builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();

            // ===== Module 2 - Circle Request Management =====
            builder.Services.AddSingleton(TimeProvider.System);
            builder.Services.AddScoped<ICircleRequestService, CircleRequestService>();
            builder.Services.AddScoped<ICircleRequestReviewService, CircleRequestReviewService>();
            builder.Services.AddScoped<CircleRegistryService>();
            builder.Services.AddScoped<ICircleRegistryService>(provider =>
                provider.GetRequiredService<CircleRegistryService>());
            builder.Services.AddScoped<ISlotAssignmentService>(provider =>
                provider.GetRequiredService<CircleRegistryService>());
            builder.Services.AddScoped<IListingAvailabilityService, ListingAvailabilityService>();
            builder.Services.AddCircleRequestManagementRepositories();

            // ===== Module 3 - Membership Applications =====
            builder.Services.AddScoped<IMembershipApplicationRepository, MembershipApplicationRepository>();
            builder.Services.AddScoped<IMembershipApplicationService, MembershipApplicationService>();

            // ===== Module 5 - Agreement & Payment =====
            builder.Services.AddScoped<IMembershipAgreementRepository, MembershipAgreementRepository>();
            builder.Services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();
            builder.Services.AddScoped<IMembershipAgreementService, MembershipAgreementService>();
            builder.Services.AddScoped<IPaymentTransactionService, PaymentTransactionService>();
            builder.Services.AddScoped<IPaymentReceiptPdfService, PaymentReceiptPdfService>();

            // ===== Module 6 - Onboarding & Member Ledger =====
            builder.Services.AddScoped<IOnboardingCaseRepository, OnboardingCaseRepository>();
            builder.Services.AddScoped<IDocumentRequirementRepository, DocumentRequirementRepository>();
            builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
            builder.Services.AddScoped<IMemberLedgerRepository, MemberLedgerRepository>();
            builder.Services.AddScoped<IDocumentService, DocumentService>();
            builder.Services.AddScoped<IDocumentRequirementService, DocumentRequirementService>();
            builder.Services.AddScoped<IOnboardingCaseService, OnboardingCaseService>();
            builder.Services.AddScoped<IMemberLedgerService, MemberLedgerService>();

            //===== Data Seeding =====
            builder.Services.AddScoped<IDataInitializer, IdentityDataInitializer>();



            // ===== Hangfire =====
            builder.Services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddHangfireServer();

            // ===== JWT Authentication =====
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var secretKey = builder.Configuration["JwtOptions:SecretKey"]!;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JwtOptions:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JwtOptions:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            // ===== Identity =====
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

            //===== Redis =====
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection")!));
            builder.Services.AddScoped<IRateLimiterService, RedisRateLimiterService>();



            var app = builder.Build();

            //===== Data Seeding =====

            await app.MigrateDatabaseAsync();
            await app.SeedDatabaseAsync();

            // ===== HTTP Request Pipeline =====
            app.UseMiddleware<ExceptionHandlerMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AngularDev");
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
            });

            app.MapControllers();

            app.Run();
        }
    }
}