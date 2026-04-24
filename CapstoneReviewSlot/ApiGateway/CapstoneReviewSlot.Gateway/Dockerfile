FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["CapstoneReviewSlot/ApiGateway/CapstoneReviewSlot.Gateway/CapstoneReviewSlot.Gateway.csproj", "ApiGateway/CapstoneReviewSlot.Gateway/"]
COPY ["CapstoneReviewSlot/GrpcContract/GrpcContract.csproj", "GrpcContract/"]
RUN dotnet restore "ApiGateway/CapstoneReviewSlot.Gateway/CapstoneReviewSlot.Gateway.csproj"

COPY CapstoneReviewSlot/ .
WORKDIR "/src/ApiGateway/CapstoneReviewSlot.Gateway"
RUN dotnet build "CapstoneReviewSlot.Gateway.csproj" -c Release -o /app/build

RUN dotnet publish "CapstoneReviewSlot.Gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CapstoneReviewSlot.Gateway.dll"]
