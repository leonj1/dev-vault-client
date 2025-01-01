# Variables
DOCKER_TEST_IMAGE := crossplatformapp-tests
TEST_CONTAINER_NAME := crossplatformapp-tests-container
GITHOOKS_DIR := .githooks
GIT_DIR := .git
PUBLISH_DIR := publish

# Build configuration
CONFIGURATION := Release
BINARY_NAME := devvault

# Detect platform
UNAME_S := $(shell uname -s)
ifeq ($(UNAME_S),Linux)
    PLATFORM_RID := linux-x64
else ifeq ($(UNAME_S),Darwin)
    PLATFORM_RID := osx-x64
else
    PLATFORM_RID := win-x64
endif

.PHONY: test clean-test install-hooks uninstall-hooks publish clean-publish

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
	@echo "Creating test results directory..."
	mkdir -p testresults/coverage && chmod -R 777 testresults
	@echo "Cleaning up any existing test containers..."
	docker rm $(TEST_CONTAINER_NAME) 2>/dev/null || true
	@echo "Building test Docker image..."
	docker build -f Dockerfile.test -t $(DOCKER_TEST_IMAGE) .
	@echo "Running tests..."
	docker run --name $(TEST_CONTAINER_NAME) -v $(shell pwd)/testresults:/app/testresults $(DOCKER_TEST_IMAGE)
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

# Build static binary for current platform
publish:
	@echo "Building static binary for $(PLATFORM_RID)..."
	@mkdir -p $(PUBLISH_DIR)
	@cd src/CrossPlatformApp.CLI && \
	dotnet publish -c $(CONFIGURATION) -r $(PLATFORM_RID) \
		--self-contained true \
		-o ../../$(PUBLISH_DIR)
	@echo "Cleaning up debug files..."
	@rm -f $(PUBLISH_DIR)/*.dbg
	@echo "Verifying binary..."
	@ls -lh $(PUBLISH_DIR)/$(BINARY_NAME)$(if $(findstring win,$(PLATFORM_RID)),.exe,)
	@echo "Testing binary..."
	@$(PUBLISH_DIR)/$(BINARY_NAME)$(if $(findstring win,$(PLATFORM_RID)),.exe,) --help

# Clean published binaries
clean-publish:
	@echo "Cleaning published binaries..."
	@rm -rf $(PUBLISH_DIR)
	@echo "Published binaries cleaned"
