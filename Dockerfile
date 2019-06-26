FROM dcreg.service.consul/prod/development-dotnet-core-sdk-common:latest

# build scripts
COPY ./fake.sh /fenvironment/
COPY ./build.fsx /fenvironment/
COPY ./paket.dependencies /fenvironment/
COPY ./paket.references /fenvironment/
COPY ./paket.lock /fenvironment/

# sources
COPY ./Environment.fsproj /fenvironment/
COPY ./src /fenvironment/src

WORKDIR /fenvironment

RUN \
    ./fake.sh build target Build no-clean

CMD ["./fake.sh", "build", "target", "Tests", "no-clean"]
