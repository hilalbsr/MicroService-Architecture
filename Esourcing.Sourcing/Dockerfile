##See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
#
## 2
## base ad�nda base iamge olu�tur.
#FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base 
#WORKDIR /app # app klas�r�n alt�ndan �al��s�n
#EXPOSE 80 # container bu hosttan host edilsin 
#
## 1
## build image
#FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS build 
#WORKDIR /src # src olu�turup onun alt�nda �al��al�m
#COPY ["Esourcing.Sourcing/Esourcing.Sourcing.csproj", "Esourcing.Sourcing/"] # kopyalayal�m
##COPY ["EventBusRabbitMQ/EventBusRabbitMQ.csproj", "EventBusRabbitMQ/"] 
#RUN dotnet restore "Esourcing.Sourcing/Esourcing.Sourcing.csproj" # restore ediyor
#COPY . .
#WORKDIR "/src/Esourcing.Sourcing" # klas�r olu�turuyor
#RUN dotnet build "Esourcing.Sourcing.csproj" -c Release -o /app/build # build ediyor.
#
#FROM build AS publish  # aya�a kald�rmak i�in publish ediyor.
#RUN dotnet publish "Esourcing.Sourcing.csproj" -c Release -o /app/publish
#
## virtual machine olu�turuyor.(IIS �zerinde gibi) kestrel �zerinde publish ediyor.
## dll aya�a kald�r�yor
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "Esourcing.Sourcing.dll"]
#
## 2 imagedan besleniyor. 1 tanesi build i�in 1 tanesi runtime projeyi �al��t�rmak i�in gerekli olan image



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