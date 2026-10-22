# syntax=docker/dockerfile:1

# Build and publish the application in the SDK image.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the project file first so dependency restore can be cached independently.
COPY ["Portfolio.Api.csproj", "./"]
RUN dotnet restore "Portfolio.Api.csproj"

COPY . ./
RUN dotnet build "Portfolio.Api.csproj" --configuration Release --no-restore
RUN dotnet publish "Portfolio.Api.csproj" --configuration Release --no-build --output /app/publish

# Keep the final image limited to the ASP.NET Core runtime and published output.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish ./

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 10000

# Render supplies PORT at runtime. Fall back to its conventional default locally.
ENTRYPOINT ["sh", "-c", "exec dotnet Portfolio.Api.dll --urls http://0.0.0.0:${PORT:-10000}"]
