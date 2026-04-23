FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Services/Assignment/Assignment.Api/Assignment.Api.csproj", "Services/Assignment/Assignment.Api/"]
COPY ["Services/Assignment/Assignment.Application/Assignment.Application.csproj", "Services/Assignment/Assignment.Application/"]
COPY ["Services/Assignment/Assignment.Domain/Assignment.Domain.csproj", "Services/Assignment/Assignment.Domain/"]
COPY ["Services/Assignment/Assignment.Infrastructure/Assignment.Infrastructure.csproj", "Services/Assignment/Assignment.Infrastructure/"]
COPY ["Services/Session/Session.Infrastructure/Session.Infrastructure.csproj", "Services/Session/Session.Infrastructure/"]
COPY ["Services/Session/Session.Domain/Session.Domain.csproj", "Services/Session/Session.Domain/"]
COPY ["Services/Identity/Identity.Infrastructure/Identity.Infrastructure.csproj", "Services/Identity/Identity.Infrastructure/"]
COPY ["Shared/CapstoneReviewSlot.Shared/CapstoneReviewSlot.Shared.csproj", "Shared/CapstoneReviewSlot.Shared/"]
COPY ["GrpcContract/GrpcContract.csproj", "GrpcContract/"]
RUN dotnet restore "Services/Assignment/Assignment.Api/Assignment.Api.csproj"

COPY Services/ Services/
COPY Shared/ Shared/
COPY GrpcContract/ GrpcContract/
WORKDIR "/src/Services/Assignment/Assignment.Api"
RUN dotnet publish "Assignment.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Assignment.Api.dll"]
