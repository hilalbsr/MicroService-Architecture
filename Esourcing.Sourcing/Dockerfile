##See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
#
## 2
## base adýnda base iamge oluþtur.
#FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base 
#WORKDIR /app # app klasörün altýndan çalýþsýn
#EXPOSE 80 # container bu hosttan host edilsin 
#
## 1
## build image
#FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS build 
#WORKDIR /src # src oluþturup onun altýnda çalýþalým
#COPY ["Esourcing.Sourcing/Esourcing.Sourcing.csproj", "Esourcing.Sourcing/"] # kopyalayalým
##COPY ["EventBusRabbitMQ/EventBusRabbitMQ.csproj", "EventBusRabbitMQ/"] 
#RUN dotnet restore "Esourcing.Sourcing/Esourcing.Sourcing.csproj" # restore ediyor
#COPY . .
#WORKDIR "/src/Esourcing.Sourcing" # klasör oluþturuyor
#RUN dotnet build "Esourcing.Sourcing.csproj" -c Release -o /app/build # build ediyor.
#
#FROM build AS publish  # ayaða kaldýrmak için publish ediyor.
#RUN dotnet publish "Esourcing.Sourcing.csproj" -c Release -o /app/publish
#
## virtual machine oluþturuyor.(IIS üzerinde gibi) kestrel üzerinde publish ediyor.
## dll ayaða kaldýrýyor
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "Esourcing.Sourcing.dll"]
#
## 2 imagedan besleniyor. 1 tanesi build için 1 tanesi runtime projeyi çalýþtýrmak için gerekli olan image



#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM mcr.microsoft.com/dotnet/aspnet:7.0-jammy AS base
#WORKDIR /app
#EXPOSE 80
#
#FROM mcr.microsoft.com/dotnet/sdk:7.0-jammy AS build
#WORKDIR /src
#COPY ["Esourcing.Sourcing/Esourcing.Sourcing.csproj", "Esourcing.Sourcing/"]
#COPY ["EventBusRabbitMQ/EventBusRabbitMQ.csproj", "EventBusRabbitMQ/"]
#RUN dotnet restore "Esourcing.Sourcing/Esourcing.Sourcing.csproj"
#COPY . .
#WORKDIR "/src/Esourcing.Sourcing"
#RUN dotnet build "Esourcing.Sourcing.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "Esourcing.Sourcing.csproj" -c Release -o /app/publish
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "Esourcing.Sourcing.dll"]



#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Esourcing.Sourcing/Esourcing.Sourcing.csproj", "Esourcing.Sourcing/"]
COPY ["EventBusRabbitMQ/EventBusRabbitMQ.csproj", "EventBusRabbitMQ/"]
RUN dotnet restore "Esourcing.Sourcing/Esourcing.Sourcing.csproj"
COPY . .
WORKDIR "/src/Esourcing.Sourcing"
RUN dotnet build "Esourcing.Sourcing.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Esourcing.Sourcing.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Esourcing.Sourcing.dll"]