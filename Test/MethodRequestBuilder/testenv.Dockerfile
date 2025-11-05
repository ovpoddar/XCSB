FROM mcr.microsoft.com/dotnet/sdk:10.0

RUN wget https://aka.ms/getvsdbgsh && \
	sh getvsdbgsh -v latest -l /vsdbg