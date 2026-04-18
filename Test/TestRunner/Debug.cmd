@echo off
cd ..\..\
docker build --progress=plain -t test-debug:latest -f .\Debug.env.Dockerfile .
docker run --rm -d -v "%cd%\TestRunner\TestResults:/workspace/TestResults" --name test-run-container test-debug:latest 
docker exec -it test-run-container bash
docker stop test-run-container