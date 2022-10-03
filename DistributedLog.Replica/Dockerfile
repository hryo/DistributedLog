#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DistributedLog.Replica/DistributedLog.Replica.csproj", "DistributedLog.Replica/"]
COPY ["DistributedLog.Contracts/DistributedLog.Contracts.csproj", "DistributedLog.Contracts/"]
RUN dotnet restore "DistributedLog.Replica/DistributedLog.Replica.csproj"
COPY . .
WORKDIR "/src/DistributedLog.Replica"
RUN dotnet build "DistributedLog.Replica.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DistributedLog.Replica.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DistributedLog.Replica.dll"]