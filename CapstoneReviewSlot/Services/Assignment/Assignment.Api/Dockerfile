FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["CapstoneReviewSlot/Services/Assignment/Assignment.Api/Assignment.Api.csproj", "Services/Assignment/Assignment.Api/"]
COPY ["CapstoneReviewSlot/Services/Assignment/Assignment.Application/Assignment.Application.csproj", "Services/Assignment/Assignment.Application/"]
COPY ["CapstoneReviewSlot/Services/Assignment/Assignment.Domain/Assignment.Domain.csproj", "Services/Assignment/Assignment.Domain/"]
COPY ["CapstoneReviewSlot/Services/Assignment/Assignment.Infrastructure/Assignment.Infrastructure.csproj", "Services/Assignment/Assignment.Infrastructure/"]
COPY ["CapstoneReviewSlot/Services/Session/Session.Infrastructure/Session.Infrastructure.csproj", "Services/Session/Session.Infrastructure/"]
COPY ["CapstoneReviewSlot/Services/Session/Session.Domain/Session.Domain.csproj", "Services/Session/Session.Domain/"]
COPY ["CapstoneReviewSlot/Services/Identity/Identity.Infrastructure/Identity.Infrastructure.csproj", "Services/Identity/Identity.Infrastructure/"]
COPY ["CapstoneReviewSlot/Shared/CapstoneReviewSlot.Shared/CapstoneReviewSlot.Shared.csproj", "Shared/CapstoneReviewSlot.Shared/"]
COPY ["CapstoneReviewSlot/GrpcContract/GrpcContract.csproj", "GrpcContract/"]
RUN dotnet restore "Services/Assignment/Assignment.Api/Assignment.Api.csproj"

COPY CapstoneReviewSlot/ .
WORKDIR "/src/Services/Assignment/Assignment.Api"
RUN dotnet build "Assignment.Api.csproj" -c Release -o /app/build

RUN dotnet publish "Assignment.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Assignment.Api.dll"]
