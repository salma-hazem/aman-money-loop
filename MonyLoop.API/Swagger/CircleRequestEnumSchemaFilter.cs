using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using MonyLoop.Domain.Constants;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MonyLoop.API.Swagger;

public sealed class CircleRequestEnumSchemaFilter : ISchemaFilter
{
    private static readonly HashSet<Type> ModuleEnums =
    [
        typeof(CircleType),
        typeof(CircleRequestStatus),
        typeof(CircleStatus),
        typeof(CircleSlotStatus),
        typeof(MarketplaceListingStatus)
    ];

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var enumType = Nullable.GetUnderlyingType(context.Type) ?? context.Type;
        if (!ModuleEnums.Contains(enumType))
        {
            return;
        }

        schema.Type = "string";
        schema.Format = null;
        schema.Enum.Clear();
        foreach (var name in Enum.GetNames(enumType))
        {
            schema.Enum.Add(new OpenApiString(name));
        }
    }
}
