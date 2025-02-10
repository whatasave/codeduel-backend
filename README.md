# Codeduel Backend


## Install DotNet

```bash
$ winget install -e --id Microsoft.DotNet.SDK.8
```

## Setup

```bash
$ dotnet restore
$ dotnet run .
```

## Migrate database data

```shell
# install dotnet-ef tools
$ dotnet tool install --global dotnet-ef

# migrate the database
$ dotnet ef migrations add <migration_name>
$ dotnet ef database update

# example
$ dotnet ef migrations add init && dotnet ef database update
```
