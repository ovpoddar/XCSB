echo off
cd ..
docker run -dit --name test-run-container test-runner:latest bash
docker cp test-run-container:/workspace/TestResults:TestResults
docker stop test-run-container
docker rm test-run-container