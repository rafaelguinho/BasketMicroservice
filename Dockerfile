#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /src
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/BasketMicroservice/BasketMicroservice.csproj", "src/BasketMicroservice/"]
COPY ["src/Infra/Infra.csproj", "src/Infra/"]
COPY ["src/Model/Model.csproj", "src/Model/"]
RUN dotnet restore "src/BasketMicroservice/BasketMicroservice.csproj"
COPY . .
WORKDIR "/src/src/BasketMicroservice"
RUN dotnet build "BasketMicroservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BasketMicroservice.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BasketMicroservice.dll"]