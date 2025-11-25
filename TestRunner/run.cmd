echo off
cd ..
docker run --rm -v "%cd%\TestRunner\TestResults:/workspace/TestResults" -e CURRENTENV="DOCKER" --name test-run-container test-runner:latest 
