#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["PrimeNumbers.PrimeRegisterer.Worker/PrimeNumbers.PrimeRegisterer.Worker.csproj", "PrimeNumbers.PrimeRegisterer.Worker/"]
COPY ["PrimeNumbers.PrimeRegisterer.Core/PrimeNumbers.PrimeRegisterer.Core.csproj", "PrimeNumbers.PrimeRegisterer.Core/"]
COPY ["NuGet.Config", "."]
RUN dotnet restore "PrimeNumbers.PrimeRegisterer.Worker/PrimeNumbers.PrimeRegisterer.Worker.csproj"
COPY . .
WORKDIR "/src/PrimeNumbers.PrimeRegisterer.Worker"
RUN dotnet build "PrimeNumbers.PrimeRegisterer.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PrimeNumbers.PrimeRegisterer.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PrimeNumbers.PrimeRegisterer.Worker.dll"]