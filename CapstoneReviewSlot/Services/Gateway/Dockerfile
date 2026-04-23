FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["CapstoneReviewSlot/Services/Gateway/Gateway.csproj", "Services/Gateway/"]
COPY ["CapstoneReviewSlot/GrpcContract/GrpcContract.csproj", "GrpcContract/"]
RUN dotnet restore "Services/Gateway/Gateway.csproj"

COPY CapstoneReviewSlot/ .
WORKDIR "/src/Services/Gateway"
RUN dotnet build "Gateway.csproj" -c Release -o /app/build

RUN dotnet publish "Gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Gateway.dll"]
