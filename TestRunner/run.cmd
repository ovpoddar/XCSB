echo off
cd ..
docker run -dit --name test-run-container test-runner:latest bash
docker exec -it test-run-container bash
