
  
FROM microsoft/dotnet:3.0-sdk as build

ARG BUILDCONFIG=RELEASE
ARG VERSION=1.0.0

COPY WebAplicationAPI1.csproj /build/
COPY ../TweetBooks.Contracts/TweetBooks.Contracts.csproj /build/TweetBooks.Contracts/
RUN dotnet restore ./build/TweetBooks.Contracts/TweetBooks.Contracts.csproj
RUN dotnet restore ./build/WebAplicationAPI1.csproj

COPY . ./build/
WORKDIR /build/
RUN dotnet publish ./WebAplicationAPI1.csproj -c $BUILDCONFIG -o out /p:Version=$VERSION

FROM microsoft/dotnet:3.0-aspnetcore-runtime
WORKDIR /app

COPY --from=build /build/out .

ENTRYPOINT ["dotnet", "WebAplicationAPI1.dll"] 