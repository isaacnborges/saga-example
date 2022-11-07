FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Payment.Worker/Payment.Worker.csproj", "src/Payment.Worker/"]
COPY ["src/Payment.Domain/Payment.Domain.csproj", "src/Payment.Domain/"]
COPY ["src/Saga.Core/Saga.Core.csproj", "src/Saga.Core/"]
RUN dotnet restore "src/Payment.Worker/Payment.Worker.csproj"
COPY . .
WORKDIR "/src/src/Payment.Worker"
RUN dotnet build "Payment.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Payment.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Payment.Worker.dll"]