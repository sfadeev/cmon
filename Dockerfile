FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
ARG TARGETARCH
WORKDIR /source

COPY Directory.Packages.props src/CMon/*.csproj .
RUN dotnet restore -a $TARGETARCH

COPY src/CMon/. .
RUN dotnet publish --no-restore -a $TARGETARCH -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine
EXPOSE 8080
WORKDIR /app
COPY --from=build /app .
# Uncomment to enable non-root user
# USER $APP_UID
ENTRYPOINT [ "./cmon" ]