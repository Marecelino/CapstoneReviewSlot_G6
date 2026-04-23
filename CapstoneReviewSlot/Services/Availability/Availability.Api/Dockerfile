FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["CapstoneReviewSlot/Services/Availability/Availability.Api/Availability.Api.csproj", "Services/Availability/Availability.Api/"]
COPY ["CapstoneReviewSlot/Services/Availability/Availability.Application/Availability.Application.csproj", "Services/Availability/Availability.Application/"]
COPY ["CapstoneReviewSlot/Services/Availability/Availability.Domain/Availability.Domain.csproj", "Services/Availability/Availability.Domain/"]
COPY ["CapstoneReviewSlot/Services/Availability/Availability.Infrastructure/Availability.Infrastructure.csproj", "Services/Availability/Availability.Infrastructure/"]
COPY ["CapstoneReviewSlot/Shared/CapstoneReviewSlot.Shared/CapstoneReviewSlot.Shared.csproj", "Shared/CapstoneReviewSlot.Shared/"]
COPY ["CapstoneReviewSlot/GrpcContract/GrpcContract.csproj", "GrpcContract/"]
RUN dotnet restore "Services/Availability/Availability.Api/Availability.Api.csproj"

COPY CapstoneReviewSlot/ .
WORKDIR "/src/Services/Availability/Availability.Api"
RUN dotnet build "Availability.Api.csproj" -c Release -o /app/build

RUN dotnet publish "Availability.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Availability.Api.dll"]
