#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["JumpMapElo.Blazor/JumpMapElo.Blazor.csproj", "JumpMapElo.Blazor/"]
RUN dotnet restore "JumpMapElo.Blazor/JumpMapElo.Blazor.csproj"
COPY . .
WORKDIR "/src/JumpMapElo.Blazor"
RUN dotnet build "JumpMapElo.Blazor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JumpMapElo.Blazor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JumpMapElo.Blazor.dll"]