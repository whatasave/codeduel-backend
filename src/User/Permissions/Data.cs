namespace Permissions;

public record Permissions(
    bool CanEditChallenges,
    bool CanEditOwnChallenges,
    bool CanEditUserPermissions
) {
    public Permissions(Entity entity) : this(
        entity.CanEditChallenges,
        entity.CanEditOwnChallenges,
        entity.CanEditUserPermissions
    ) {}

    public static Permissions FromCompactNotation(int compactNotation) {
        Type[] constructorParams = typeof(Permissions).GetFields().Select(field => field.FieldType).ToArray();
        var constructor = typeof(Permissions).GetConstructor(constructorParams)!;
        return (Permissions)constructor.Invoke(
            typeof(Permissions)
                .GetFields()
                .Select((field, i) => (field, i))
                .Aggregate(new object[constructorParams.Length], (acc, fieldAndIndex) => {
                    var (field, i) = fieldAndIndex;
                    acc[i] = (compactNotation & 1) == 1;
                    compactNotation >>= 1;
                    return acc;
                })
        );
    }

    public int ToCompactNotation() {
        return GetType()
            .GetFields()
            .Aggregate(0, (acc, permission) => (acc << 1) | ((bool)permission.GetValue(this)! ? 1 : 0));
    }
}

public record UserPermissions(
    int CompactNotation,
    Permissions Permissions
) {
    public UserPermissions(int compactNotation) : this(compactNotation, Permissions.FromCompactNotation(compactNotation)) { }
    public UserPermissions(Entity entity) : this(entity.CompactNotation, new(entity)) { }
}
