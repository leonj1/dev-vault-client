# Variables
DOCKER_TEST_IMAGE := crossplatformapp-tests
TEST_CONTAINER_NAME := crossplatformapp-tests-container
GITHOOKS_DIR := .githooks
GIT_DIR := .git

.PHONY: test clean-test install-hooks uninstall-hooks

# Install git hooks
install-hooks:
	@echo "Installing git hooks..."
	@git config core.hooksPath $(GITHOOKS_DIR)
	@echo "Git hooks installed successfully"

# Uninstall git hooks
uninstall-hooks:
	@echo "Uninstalling git hooks..."
	@git config --unset core.hooksPath
	@echo "Git hooks uninstalled successfully"

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

# Initialize project (install hooks and restore packages)
init: install-hooks
	@echo "Initializing project..."
	@cd src && dotnet restore
	@echo "Project initialized successfully"
