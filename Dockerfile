# Use the SDK image to build the solution
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy the solution and project files and restore any dependencies (NuGet packages)
COPY ["TestAPI/TestAPI.sln", "./"]
COPY ["TestAPI/TestAPI/TestAPI.csproj", "./TestAPI/"]
RUN dotnet restore "TestAPI/TestAPI.csproj"

# Copy the rest of the source code and publish the application
COPY ["TestAPI", "./TestAPI/"]
RUN dotnet publish "TestAPI/TestAPI.sln" -c Release -o /app/publish

# Use the runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "TestAPI.dll"]
