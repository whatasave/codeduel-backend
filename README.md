## Migrate database data

```shell
$ dotnet ef migrations add <migration_name>
$ dotnet ef database update

# example
$ dotnet ef migrations add init && dotnet ef database update
```
