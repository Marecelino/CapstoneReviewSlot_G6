FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["CapstoneReviewSlot/Services/Registration/Registration.Api/Registration.Api.csproj", "Services/Registration/Registration.Api/"]
COPY ["CapstoneReviewSlot/Services/Registration/Registration.Application/Registration.Application.csproj", "Services/Registration/Registration.Application/"]
COPY ["CapstoneReviewSlot/Services/Registration/Registration.Domain/Registration.Domain.csproj", "Services/Registration/Registration.Domain/"]
COPY ["CapstoneReviewSlot/Services/Registration/Registration.Infrastructure/Registration.Infrastructure.csproj", "Services/Registration/Registration.Infrastructure/"]
COPY ["CapstoneReviewSlot/Services/Session/Session.Infrastructure/Session.Infrastructure.csproj", "Services/Session/Session.Infrastructure/"]
COPY ["CapstoneReviewSlot/Services/Session/Session.Domain/Session.Domain.csproj", "Services/Session/Session.Domain/"]
COPY ["CapstoneReviewSlot/Services/Identity/Identity.Infrastructure/Identity.Infrastructure.csproj", "Services/Identity/Identity.Infrastructure/"]
COPY ["CapstoneReviewSlot/Shared/CapstoneReviewSlot.Shared/CapstoneReviewSlot.Shared.csproj", "Shared/CapstoneReviewSlot.Shared/"]
COPY ["CapstoneReviewSlot/GrpcContract/GrpcContract.csproj", "GrpcContract/"]
RUN dotnet restore "Services/Registration/Registration.Api/Registration.Api.csproj"

COPY CapstoneReviewSlot/ .
WORKDIR "/src/Services/Registration/Registration.Api"
RUN dotnet build "Registration.Api.csproj" -c Release -o /app/build

RUN dotnet publish "Registration.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Registration.Api.dll"]
