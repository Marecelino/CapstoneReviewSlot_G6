FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Services/Session/Session.Api/Session.Api.csproj", "Services/Session/Session.Api/"]
COPY ["Services/Session/Session.Application/Session.Application.csproj", "Services/Session/Session.Application/"]
COPY ["Services/Session/Session.Domain/Session.Domain.csproj", "Services/Session/Session.Domain/"]
COPY ["Services/Session/Session.Infrastructure/Session.Infrastructure.csproj", "Services/Session/Session.Infrastructure/"]
COPY ["Services/Identity/Identity.Infrastructure/Identity.Infrastructure.csproj", "Services/Identity/Identity.Infrastructure/"]
COPY ["Shared/CapstoneReviewSlot.Shared/CapstoneReviewSlot.Shared.csproj", "Shared/CapstoneReviewSlot.Shared/"]
COPY ["GrpcContract/GrpcContract.csproj", "GrpcContract/"]
RUN dotnet restore "Services/Session/Session.Api/Session.Api.csproj"

COPY Services/ Services/
COPY Shared/ Shared/
COPY GrpcContract/ GrpcContract/
WORKDIR "/src/Services/Session/Session.Api"
RUN dotnet publish "Session.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Session.Api.dll"]
