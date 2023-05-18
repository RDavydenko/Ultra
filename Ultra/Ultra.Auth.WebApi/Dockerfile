#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Ultra.Auth.WebApi/Ultra.Auth.WebApi.csproj", "Ultra.Auth.WebApi/"]
RUN dotnet restore "Ultra.Auth.WebApi/Ultra.Auth.WebApi.csproj"
COPY . .
WORKDIR "/src/Ultra.Auth.WebApi"
RUN dotnet build "Ultra.Auth.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ultra.Auth.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ultra.Auth.WebApi.dll"]