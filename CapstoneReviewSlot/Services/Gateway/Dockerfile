FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Services/Gateway/Gateway.csproj", "Services/Gateway/"]
COPY ["GrpcContract/GrpcContract.csproj", "GrpcContract/"]
RUN dotnet restore "Services/Gateway/Gateway.csproj"

COPY Services/ Services/
COPY Shared/ Shared/
COPY GrpcContract/ GrpcContract/
WORKDIR "/src/Services/Gateway"
RUN dotnet publish "Gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Gateway.dll"]
