FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 3306

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src

COPY ["Api/Api.csproj", "Api/"]
COPY ["Api.Core/Api.Core.csproj", "Api.Core/"]
COPY ["Api.Services/Api.Services.csproj", "Api.Services/"]
COPY ["Api.Models/Api.Models.csproj", "Api.Models/"]
COPY ["Api.Provider/Api.Provider.csproj", "Api.Provider/"]
COPY ["Api.Dal/Api.Dal.csproj", "Api.Dal/"]
COPY ["Api.Utils/Api.Utils.csproj", "Api.Utils/"]
COPY ["Api.Factories/Api.Factories.csproj", "Api/Factories"]

RUN dotnet restore "Api/Api.csproj"
COPY . .

WORKDIR "/src/Api"
RUN dotnet build "Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]