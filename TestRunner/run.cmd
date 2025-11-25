echo off
cd ..
b docker run --rm -v "%cd%\TestRunner\TestResults:/workspace/TestResults" --name test-run-container test-runner:latest 
