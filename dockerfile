# temprorary/Staging BOX to prepare and test
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Copy the solution file first
COPY ["Library.sln", "."]
# Copy the project files
COPY ["Library.API/Library.csproj", "Library.API/"]

# Copy the source code for each project
COPY ["Library.API/", "Library.API/"]
COPY ["Library.Domain/", "Library.Domain/"]
COPY ["Library.Services/", "Library.Services/"]
COPY ["Library.Repository/", "Library.Repository/"]


RUN dotnet restore "Library.API/Library.csproj"

WORKDIR /src/Library.API
RUN dotnet build "Library.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Library.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final 
WORKDIR /app
COPY --from=publish /app/publish .
# dll will be generated using .csproj file name. Library.csproj=>Library.dll
ENTRYPOINT [ "dotnet", "Library.dll"] 


# now run docker build -t libraryapi .
# then run docker run -e ASPNETCORE_ENVIRONMENT=Development -p 8080:8080 libraryapi