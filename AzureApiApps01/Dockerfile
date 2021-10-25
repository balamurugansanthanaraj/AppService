#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["AzureApiApps01.csproj", "AzureApiApps01/"]
RUN dotnet restore "AzureApiApps01/AzureApiApps01.csproj"
WORKDIR "/src/AzureApiApps01"
COPY . .
RUN ls -ltr
RUN dotnet build "AzureApiApps01.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AzureApiApps01.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AzureApiApps01.dll"]