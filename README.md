# BLUE16 clieeent

A lightweight Windows client for the BLUE16 launcher.

## Requirements
- .NET 8 SDK
- Windows (UI uses WinForms because i am so bad)

## Build
Open the solution in Visual Studio or use the dotnet CLI:

```bash
dotnet restore
dotnet build -c Release
```

## Run
From Visual Studio, set `BLUE16Client` as the startup project and run. Or:

```bash
dotnet run --project BLUE16Client/BLUE16Client.csproj
```

## Contributing
See `CONTRIBUTING.md` for contribution guidelines.
