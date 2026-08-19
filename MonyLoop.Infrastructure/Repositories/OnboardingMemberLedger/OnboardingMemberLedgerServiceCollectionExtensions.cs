using Microsoft.Extensions.DependencyInjection;
using MonyLoop.Domain.Interfaces;
using MonyLoop.Domain.Interfaces.OnboardingMemberLedger;
using MonyLoop.Domain.Interfaces.UserAuth;
using MonyLoop.Infrastructure.Repositories.UserAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Infrastructure.Repositories.OnboardingMemberLedger
{
    public static class OnboardingMemberLedgerServiceCollectionExtensions
    {
        public static IServiceCollection AddOnboardingMemberLedgerRepositories(
            this IServiceCollection services)
        {
            services.AddScoped<IOnboardingCaseRepository, OnboardingCaseRepository>();
            services.AddScoped<IDocumentRequirementRepository, DocumentRequirementRepository>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<IMemberLedgerRepository, MemberLedgerRepository>();
            services.AddScoped<IOTPTokenRepository, OTPTokenRepository>();

            return services;
        }
    }
}
