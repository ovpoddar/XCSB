cd ../../
docker run --rm -v "$(pwd)\TestRunner\TestResults:/workspace/TestResults" --name test-run-container test-runner:latest 