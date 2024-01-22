ARG FROM_REPO=mcr.microsoft.com/dotnet
ARG DOTNET_SDK_IMAGE=sdk:sdk:8.0-jammy
ARG ASPNET_RUNTIME_IMAGE=aspnet:aspnet:8.0-jammy
ARG RUNTIME_IDENTIFIER=linux-x64

FROM ${FROM_REPO}/${DOTNET_SDK_IMAGE} as sdk-image
FROM ${FROM_REPO}/${ASPNET_RUNTIME_IMAGE} as runtime-image

##
## Build
##
FROM sdk-image AS build

ARG RUNTIME_IDENTIFIER

RUN mkdir /app
WORKDIR /app

# 1 - Copy projects
COPY samples/Samples.WebApi/Samples.WebApi.csproj samples/Samples.WebApi/
COPY ./*/*.csproj ./

#Restore original file paths
RUN for file in $(ls *.csproj); do mkdir -p ./${file%.*}/ && mv $file ./${file%.*}/ && echo $file; done

# 1.1 - Restore packages
RUN dotnet restore samples/Samples.WebApi/Samples.WebApi.csproj -r $RUNTIME_IDENTIFIER

# 2 - Copy all files
COPY . .

# 3 - Build
RUN dotnet build -c Release samples/Samples.WebApi/Samples.WebApi.csproj

##
## Publish
##
FROM build AS publish
RUN dotnet publish samples/Samples.WebApi/Samples.WebApi.csproj --no-build -c Release -o /out

##
## Run
##
FROM runtime-image

COPY --from=publish /out /out
WORKDIR /out
ENV ASPNETCORE_URLS=http://+:5261

COPY --from=publish /out .
ENTRYPOINT ["dotnet", "Samples.WebApi.dll"]