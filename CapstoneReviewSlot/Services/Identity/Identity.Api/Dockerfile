FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["CapstoneReviewSlot/Services/Identity/Identity.Api/Identity.Api.csproj", "Services/Identity/Identity.Api/"]
COPY ["CapstoneReviewSlot/Services/Identity/Identity.Application/Identity.Application.csproj", "Services/Identity/Identity.Application/"]
COPY ["CapstoneReviewSlot/Services/Identity/Identity.Domain/Identity.Domain.csproj", "Services/Identity/Identity.Domain/"]
COPY ["CapstoneReviewSlot/Services/Identity/Identity.Infrastructure/Identity.Infrastructure.csproj", "Services/Identity/Identity.Infrastructure/"]
COPY ["CapstoneReviewSlot/Shared/CapstoneReviewSlot.Shared/CapstoneReviewSlot.Shared.csproj", "Shared/CapstoneReviewSlot.Shared/"]
COPY ["CapstoneReviewSlot/GrpcContract/GrpcContract.csproj", "GrpcContract/"]
RUN dotnet restore "Services/Identity/Identity.Api/Identity.Api.csproj"

COPY CapstoneReviewSlot/ .
WORKDIR "/src/Services/Identity/Identity.Api"
RUN dotnet build "Identity.Api.csproj" -c Release -o /app/build

RUN dotnet publish "Identity.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Identity.Api.dll"]
