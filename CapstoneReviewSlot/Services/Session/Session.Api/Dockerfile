FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["CapstoneReviewSlot/Services/Session/Session.Api/Session.Api.csproj", "Services/Session/Session.Api/"]
COPY ["CapstoneReviewSlot/Services/Session/Session.Application/Session.Application.csproj", "Services/Session/Session.Application/"]
COPY ["CapstoneReviewSlot/Services/Session/Session.Domain/Session.Domain.csproj", "Services/Session/Session.Domain/"]
COPY ["CapstoneReviewSlot/Services/Session/Session.Infrastructure/Session.Infrastructure.csproj", "Services/Session/Session.Infrastructure/"]
COPY ["CapstoneReviewSlot/Services/Identity/Identity.Infrastructure/Identity.Infrastructure.csproj", "Services/Identity/Identity.Infrastructure/"]
COPY ["CapstoneReviewSlot/Shared/CapstoneReviewSlot.Shared/CapstoneReviewSlot.Shared.csproj", "Shared/CapstoneReviewSlot.Shared/"]
COPY ["CapstoneReviewSlot/GrpcContract/GrpcContract.csproj", "GrpcContract/"]
RUN dotnet restore "Services/Session/Session.Api/Session.Api.csproj"

COPY CapstoneReviewSlot/ .
WORKDIR "/src/Services/Session/Session.Api"
RUN dotnet build "Session.Api.csproj" -c Release -o /app/build

RUN dotnet publish "Session.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Session.Api.dll"]
