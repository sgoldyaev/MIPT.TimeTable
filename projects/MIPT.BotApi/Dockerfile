FROM mcr.microsoft.com/dotnet/sdk

COPY / ./src

RUN dotnet publish /src/MIPT.TimeTable.sln -c Debug -o /app 

WORKDIR /app

ENTRYPOINT ["dotnet", "MIPT.BotApi.dll"]