FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY semestrovka1_oris.csproj ./
RUN dotnet restore semestrovka1_oris.csproj
COPY . .
RUN dotnet publish semestrovka1_oris.csproj -c Release --no-restore -o /out

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build --chown=app:app /out/ ./
USER app
ENV ORIS_HTTP_PREFIX=http://+:8888/
EXPOSE 8888
ENTRYPOINT ["dotnet", "semestrovka1_oris.dll"]
