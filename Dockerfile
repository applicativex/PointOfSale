# build stage
FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /app

# copy proj files needed for package restore
COPY *.sln .
COPY src/PointOfSale.Domain/*.csproj ./src/PointOfSale.Domain/
COPY src/PointOfSale.Api/*.csproj ./src/PointOfSale.Api/
COPY test/PointOfSale.Tests/*.csproj ./test/PointOfSale.Tests/
COPY test/PointOfSale.IntegrationTests/*.csproj ./test/PointOfSale.IntegrationTests/

# restore packages
RUN dotnet restore

# copy everything else
COPY . .

# run tests
RUN dotnet test test/PointOfSale.Tests/PointOfSale.Tests.csproj
RUN dotnet test test/PointOfSale.IntegrationTests/PointOfSale.IntegrationTests.csproj

# build app
WORKDIR /app/src/PointOfSale.Api/
RUN dotnet publish -c Release -o out

# runtime stage
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
WORKDIR /app
COPY --from=build /app/src/PointOfSale.Api/out ./
RUN ls

ENTRYPOINT ["dotnet", "PointOfSale.Api.dll"]