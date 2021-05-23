FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Notify/Notify.csproj", "Notify/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ["Services/Services.csproj", "Services/"]
COPY ["Context/Context.csproj", "Context/"]
RUN dotnet restore "Notify/Notify.csproj"
COPY . .
WORKDIR "/src/Notify"
RUN dotnet build "Notify.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Notify.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notify.dll"]