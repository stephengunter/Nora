dotnet build ./Nora.sln && dotnet run --project ./src/Web
dotnet build ./Nora.sln
dotnet run --project ./Web
dotnet ef database update -s ../Web
dotnet ef migrations add InitCreate -s ../Web
dotnet ef migrations remove -s ../Web