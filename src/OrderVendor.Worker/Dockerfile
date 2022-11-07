FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/OrderVendor.Worker/OrderVendor.Worker.csproj", "src/OrderVendor.Worker/"]
COPY ["src/Saga.Core/Saga.Core.csproj", "src/Saga.Core/"]
RUN dotnet restore "src/OrderVendor.Worker/OrderVendor.Worker.csproj"
COPY . .
WORKDIR "/src/src/OrderVendor.Worker"
RUN dotnet build "OrderVendor.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderVendor.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderVendor.Worker.dll"]