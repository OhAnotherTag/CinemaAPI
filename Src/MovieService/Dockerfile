FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MovieService/MovieService.csproj", "MovieService/"]
COPY ["Domain/Domain.csproj", "Domain/"]
RUN dotnet restore "MovieService/MovieService.csproj"
COPY . .
WORKDIR "/src/MovieService"
RUN dotnet build "MovieService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MovieService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MovieService.dll"]
