# Variables
DOCKER_TEST_IMAGE := crossplatformapp-tests
TEST_CONTAINER_NAME := crossplatformapp-tests-container

.PHONY: test clean-test

# Build test image and run tests
test:
	@echo "Cleaning up any existing test containers..."
	docker rm $(TEST_CONTAINER_NAME) 2>/dev/null || true
	@echo "Building test Docker image..."
	docker build -f Dockerfile.test -t $(DOCKER_TEST_IMAGE) .
	@echo "Running tests..."
	docker run --name $(TEST_CONTAINER_NAME) $(DOCKER_TEST_IMAGE)
	@echo "Copying test results..."
	docker cp $(TEST_CONTAINER_NAME):/app/testresults ./testresults
	@echo "Cleaning up..."
	docker rm $(TEST_CONTAINER_NAME)
	@echo "Test execution complete. Results available in ./testresults"

# Clean up test artifacts
clean-test:
	@echo "Cleaning up test artifacts..."
	docker rmi $(DOCKER_TEST_IMAGE) 2>/dev/null || true
	docker rm $(TEST_CONTAINER_NAME) 2>/dev/null || true
	rm -rf ./testresults
	@echo "Clean up complete"
