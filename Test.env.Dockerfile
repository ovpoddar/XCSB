FROM ubuntu:24.04 AS operating-system
ENV DEBIAN_FRONTEND=noninteractive
RUN apt-get update && apt-get install -y xvfb libx11-dev libxext-dev libxrender-dev libxrandr-dev libxfixes-dev libxi-dev\
    libxcursor-dev libxtst-dev && rm -rf /var/lib/apt/lists/*

FROM operating-system AS c-lang
RUN apt-get update && apt-get install -y build-essential curl vim libxcb1-dev gcc\
    && rm -rf /var/lib/apt/lists/*

FROM c-lang AS cs-lang
WORKDIR /temp
RUN apt-get update && apt-get install -y zlib1g ca-certificates libc6 libgcc-s1 libicu74 liblttng-ust1 libssl3 libstdc++6 \
    && rm -rf /var/lib/apt/lists/*
RUN curl --request GET -sSL --url 'https://builds.dotnet.microsoft.com/dotnet/Sdk/9.0.306/dotnet-sdk-9.0.306-linux-x64.tar.gz'\
    --output 'dotnet-sdk-9.linux.tar.gz'
RUN mkdir -p /usr/share/dotnet && tar zxf dotnet-sdk-9.linux.tar.gz -C /usr/share/dotnet \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet
RUN curl --request GET -sSL --url 'https://builds.dotnet.microsoft.com/dotnet/Sdk/10.0.100/dotnet-sdk-10.0.100-linux-x64.tar.gz'\
    --output './dotnet-sdk-10.linux.tar.gz'
RUN tar zxf dotnet-sdk-10.linux.tar.gz -C /usr/share/dotnet
RUN rm -rf /temp

FROM cs-lang AS content
WORKDIR /workspace
COPY . .
WORKDIR /workspace/Src/
RUN dotnet build -c Release

WORKDIR /workspace/Test/MethodRequestBuilder
ENV DISPLAY=:0
ENV SCREEN_RESOLUTION=1280x720x24
ENV TEST_RESULTS_DIR=/workspace/TestResults

RUN mkdir -p ${TEST_RESULTS_DIR}

RUN dotnet build -p:DOCKER=true
ENTRYPOINT ["bash", "-c", "xvfb-run -s '-screen 0 1280x720x24' dotnet test --logger 'trx;LogFileName=test_results.trx' --results-directory $TEST_RESULTS_DIR; exit 0"]
