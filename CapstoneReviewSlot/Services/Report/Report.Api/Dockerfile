FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["CapstoneReviewSlot/Services/Report/Report.Api/Report.Api.csproj", "Services/Report/Report.Api/"]
COPY ["CapstoneReviewSlot/Services/Report/Report.Application/Report.Application.csproj", "Services/Report/Report.Application/"]
COPY ["CapstoneReviewSlot/Services/Report/Report.Domain/Report.Domain.csproj", "Services/Report/Report.Domain/"]
COPY ["CapstoneReviewSlot/Services/Report/Report.Infrastructure/Report.Infrastructure.csproj", "Services/Report/Report.Infrastructure/"]
COPY ["CapstoneReviewSlot/Services/Session/Session.Infrastructure/Session.Infrastructure.csproj", "Services/Session/Session.Infrastructure/"]
COPY ["CapstoneReviewSlot/Services/Session/Session.Domain/Session.Domain.csproj", "Services/Session/Session.Domain/"]
COPY ["CapstoneReviewSlot/Shared/CapstoneReviewSlot.Shared/CapstoneReviewSlot.Shared.csproj", "Shared/CapstoneReviewSlot.Shared/"]
COPY ["CapstoneReviewSlot/GrpcContract/GrpcContract.csproj", "GrpcContract/"]
RUN dotnet restore "Services/Report/Report.Api/Report.Api.csproj"

COPY CapstoneReviewSlot/ .
WORKDIR "/src/Services/Report/Report.Api"
RUN dotnet build "Report.Api.csproj" -c Release -o /app/build

RUN dotnet publish "Report.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Report.Api.dll"]
