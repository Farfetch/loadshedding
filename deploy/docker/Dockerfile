ARG FROM_REPO=mcr.microsoft.com/dotnet
ARG DOTNET_SDK_IMAGE=sdk:8.0-jammy
ARG DOTNET_RUNTIME_IMAGE=runtime:8.0-jammy
ARG ASPNET_RUNTIME_IMAGE=aspnet:8.0-jammy
ARG RUNTIME_IDENTIFIER=linux-x64

##
## sdk-image - Has the .net core SDK required to build and run tests
##
FROM ${FROM_REPO}/${DOTNET_SDK_IMAGE} as sdk-image

##
## runtime-image - Has the .net core runtime, required to run the service
##
FROM ${FROM_REPO}/${DOTNET_RUNTIME_IMAGE} as runtime-image

##
## aspnet-runtime-image - Has the asp.net core runtime, required to run the web applications
##
FROM ${FROM_REPO}/${ASPNET_RUNTIME_IMAGE} AS aspnet-runtime-image

##
## Building
##
FROM sdk-image AS build

ARG RUNTIME_IDENTIFIER

RUN mkdir /app
WORKDIR /app

# 1 - Copy sln and csprojs to make restore faster - Optional
COPY LoadShedding.sln LoadShedding.sln

# 1.1 Library
COPY src/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done

# 1.2 Unit tests
COPY tests/unit-tests/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p tests/unit-tests/${file%.*}/ && mv $file tests/unit-tests/${file%.*}/; done

# 1.3 Integration tests
COPY tests/integration-tests/Farfetch.LoadShedding.IntegrationTests/Farfetch.LoadShedding.IntegrationTests.csproj tests/integration-tests/Farfetch.LoadShedding.IntegrationTests/Farfetch.LoadShedding.IntegrationTests.csproj

# 1.4 Performance tests
COPY tests/performance-tests/Farfetch.LoadShedding.PerformanceTests/Farfetch.LoadShedding.PerformanceTests.csproj tests/performance-tests/Farfetch.LoadShedding.PerformanceTests/Farfetch.LoadShedding.PerformanceTests.csproj

# 1.5 - Restore nuget packages
RUN dotnet restore LoadShedding.sln -r ${RUNTIME_IDENTIFIER}

# 2 - Copy all files
COPY . .

# 3 - Build
RUN dotnet build --no-restore --configuration Release LoadShedding.sln
RUN mkdir /reports

# 5 - Publish performance tests sample

RUN dotnet publish tests/performance-tests/Farfetch.LoadShedding.PerformanceTests/Farfetch.LoadShedding.PerformanceTests.csproj -c Release -o /out --no-restore

##
## Unit Tests
##
FROM sdk-image AS unit-tests

COPY --from=build /app /app
COPY --from=build /.nuget /.nuget

WORKDIR /app

RUN dotnet test LoadShedding.sln --filter Category=Unit --configuration Release

#
# Integration Tests
#
FROM sdk-image AS integration-tests
ENV ASPNETCORE_ENVIRONMENT=Docker

COPY --from=build /app /app
COPY --from=build /.nuget /.nuget

WORKDIR /app

CMD ["dotnet", "test", "--no-restore", "tests/integration-tests/Farfetch.LoadShedding.IntegrationTests/Farfetch.LoadShedding.IntegrationTests.csproj", "--logger", "trx", "--results-directory", "/reports"]

#
# Performance Tests Sample
#
FROM aspnet-runtime-image AS performance-tests-sample
ENV ASPNETCORE_ENVIRONMENT=Docker

COPY --from=build /.nuget /.nuget
COPY --from=build /out /app

WORKDIR /app

ENV ASPNETCORE_URLS=http://+:9025 \
    ASPNETCORE_ENVIRONMENT=Docker

CMD dotnet Farfetch.LoadShedding.PerformanceTests.dll

##
## Jmeter Performance Tests
##
FROM justb4/jmeter:5.5 As performance-tests
COPY --from=build /app/tests/performance-tests/AdaptiveLimiterTests.jmx /app
WORKDIR /app

CMD ["jmeter", "-Jjmeter.save.saveservice.output_format=xml", "-JServerEndpoint=performance-tests-sample", "-JServerEndpointNoLimiter=performance-tests-sample-no-limiter", "-n", "-t", "AdaptiveLimiterTests.jmx", "-l", "results.jtl"]
