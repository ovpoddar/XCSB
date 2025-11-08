echo off
cd ../
docker build -t test-runner:latest -f .\Test.env.Dockerfile .