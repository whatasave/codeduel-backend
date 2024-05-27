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
