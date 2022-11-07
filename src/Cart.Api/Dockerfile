FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Cart.Api/Cart.Api.csproj", "src/Cart.Api/"]
COPY ["src/Saga.Core/Saga.Core.csproj", "src/Saga.Core/"]
RUN dotnet restore "src/Cart.Api/Cart.Api.csproj"
COPY . .
WORKDIR "/src/src/Cart.Api"
RUN dotnet build "Cart.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cart.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cart.Api.dll"]