using Microsoft.Extensions.DependencyInjection;
using MonyLoop.Domain.Interfaces;
using MonyLoop.Domain.Interfaces.OnboardingMemberLedger;
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

            return services;
        }
    }
}
