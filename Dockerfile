FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BitsmapWorkerService.csproj", "./"]
RUN dotnet restore "BitsmapWorkerService.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "BitsmapWorkerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BitsmapWorkerService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BitsmapWorkerService.dll"]
