FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Order.Worker/Order.Worker.csproj", "src/Order.Worker/"]
COPY ["src/Order.Domain/Order.Domain.csproj", "src/Order.Domain/"]
COPY ["src/Saga.Core/Saga.Core.csproj", "src/Saga.Core/"]
RUN dotnet restore "src/Order.Worker/Order.Worker.csproj"
COPY . .
WORKDIR "/src/src/Order.Worker"
RUN dotnet build "Order.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Order.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Order.Worker.dll"]
