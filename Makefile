# Variables
DOCKER_TEST_IMAGE := crossplatformapp-tests
TEST_CONTAINER_NAME := crossplatformapp-tests-container
GITHOOKS_DIR := .githooks
GIT_DIR := .git
PUBLISH_DIR := publish

# Platform-specific variables
LINUX_RID := linux-x64
WINDOWS_RID := win-x64
MACOS_RID := osx-x64

# Build configuration
CONFIGURATION := Release
BINARY_NAME := CrossPlatformApp

.PHONY: test clean-test install-hooks uninstall-hooks publish-linux publish-windows publish-macos clean-publish

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

# Build static binary for Linux
publish-linux:
	@echo "Building Linux binary..."
	@mkdir -p $(PUBLISH_DIR)/$(LINUX_RID)
	@cd src/CrossPlatformApp.CLI && \
	dotnet publish -c $(CONFIGURATION) -r $(LINUX_RID) \
		--self-contained true \
		-p:PublishSingleFile=true \
		-o ../../$(PUBLISH_DIR)/$(LINUX_RID)
	@echo "Linux binary built at $(PUBLISH_DIR)/$(LINUX_RID)/$(BINARY_NAME)"

# Build static binary for Windows
publish-windows:
	@echo "Building Windows binary..."
	@mkdir -p $(PUBLISH_DIR)/$(WINDOWS_RID)
	@cd src/CrossPlatformApp.CLI && \
	dotnet publish -c $(CONFIGURATION) -r $(WINDOWS_RID) \
		--self-contained true \
		-p:PublishSingleFile=true \
		-o ../../$(PUBLISH_DIR)/$(WINDOWS_RID)
	@echo "Windows binary built at $(PUBLISH_DIR)/$(WINDOWS_RID)/$(BINARY_NAME).exe"

# Build static binary for macOS
publish-macos:
	@echo "Building macOS binary..."
	@mkdir -p $(PUBLISH_DIR)/$(MACOS_RID)
	@cd src/CrossPlatformApp.CLI && \
	dotnet publish -c $(CONFIGURATION) -r $(MACOS_RID) \
		--self-contained true \
		-p:PublishSingleFile=true \
		-o ../../$(PUBLISH_DIR)/$(MACOS_RID)
	@echo "macOS binary built at $(PUBLISH_DIR)/$(MACOS_RID)/$(BINARY_NAME)"

# Build for all platforms
publish-all: publish-linux publish-windows publish-macos

# Clean published binaries
clean-publish:
	@echo "Cleaning published binaries..."
	@rm -rf $(PUBLISH_DIR)
	@echo "Published binaries cleaned"
