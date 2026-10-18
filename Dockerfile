FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["WalletManagement.Api.csproj", "./"]
RUN dotnet restore "WalletManagement.Api.csproj"
COPY . .
RUN dotnet publish "WalletManagement.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WalletManagement.Api.dll"]