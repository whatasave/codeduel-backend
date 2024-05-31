using Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class RemoveUndefinedSchemaFilter : ISchemaFilter {
    public void Apply(OpenApiSchema schema, SchemaFilterContext context) {
        if (schema.Properties == null)
            return;

        foreach (var propSchema in schema.Properties) {
            var propertyInfo = context.Type.GetProperties()
                .FirstOrDefault(p => string.Equals(p.Name, propSchema.Key, StringComparison.OrdinalIgnoreCase));

            if (propertyInfo == null)
                continue;

            if (!IsNullableType(propertyInfo.PropertyType)) {
                schema.Required ??= new HashSet<string>();
                schema.Required.Add(propSchema.Key);
            }
        }
    }

    private static bool IsNullableType(Type type) {
        return Nullable.GetUnderlyingType(type) != null;
    }
}

public class AuthOperationFilter : IOperationFilter {
    public void Apply(OpenApiOperation operation, OperationFilterContext context) {
        var hasAuthFilter = context.MethodInfo.GetCustomAttributes(inherit: true)
            .Any(a => a.GetType() == typeof(ServiceFilterAttribute) && ((ServiceFilterAttribute)a).ServiceType == typeof(AuthFilter));

        if (hasAuthFilter) {
            operation.Responses.Add("401", new OpenApiResponse {
                Description = "Unauthorized"
            });
        }
    }
}
