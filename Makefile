# Variables
DOCKER_TEST_IMAGE := crossplatformapp-tests
TEST_CONTAINER_NAME := crossplatformapp-tests-container
GITHOOKS_DIR := .githooks
GIT_DIR := .git
PUBLISH_DIR := publish
TEST_RESULTS_DIR := testresults
COVERAGE_DIR := $(TEST_RESULTS_DIR)/coverage
SRC_DIR := src
CLI_DIR := $(SRC_DIR)/CrossPlatformApp.CLI
DOCKER_BUILD_FILE := Dockerfile.test

# Build configuration
CONFIGURATION := Release
BINARY_NAME := devvault

# Platform configuration
UNAME_S := $(shell uname -s)
PLATFORM_MAP := Linux:linux-x64 Darwin:osx-x64 *:win-x64
PLATFORM_RID := $(or $(word 2,$(subst :, ,$(filter $(UNAME_S):%,$(PLATFORM_MAP)))),win-x64)

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
	mkdir -p $(COVERAGE_DIR) && chmod -R 777 $(TEST_RESULTS_DIR)
	@echo "Cleaning up any existing test containers..."
	docker rm $(TEST_CONTAINER_NAME) 2>/dev/null || true
	@echo "Building test Docker image..."
	docker build -f $(DOCKER_BUILD_FILE) -t $(DOCKER_TEST_IMAGE) \
	--build-arg USER_ID=$(shell id -u), --build-arg GROUP_ID=$(shell id -g) .
	@echo "Running tests..."
	docker run --name $(TEST_CONTAINER_NAME) \
	-v $(shell pwd)/$(TEST_RESULTS_DIR):/app/$(TEST_RESULTS_DIR) $(DOCKER_TEST_IMAGE)
	@echo "Cleaning up..."
	docker rm $(TEST_CONTAINER_NAME)
	@echo "Test execution complete. Results available in ./$(TEST_RESULTS_DIR)"

# Clean up test artifacts
clean-test:
	@echo "Cleaning up test artifacts..."
	docker rmi $(DOCKER_TEST_IMAGE) 2>/dev/null || true
	docker rm $(TEST_CONTAINER_NAME) 2>/dev/null || true
	rm -rf ./$(TEST_RESULTS_DIR)
	@echo "Clean up complete"

# Initialize project (install hooks and restore packages)
init: install-hooks
	@echo "Initializing project..."
	@cd $(SRC_DIR) && dotnet restore
	@echo "Project initialized successfully"

# Build static binary for current platform
publish:
	@echo "Building static binary for $(PLATFORM_RID)..."
	@mkdir -p $(PUBLISH_DIR)
	@cd $(CLI_DIR) && dotnet publish -c $(CONFIGURATION) -r $(PLATFORM_RID) \
	--self-contained true -o ../../$(PUBLISH_DIR)
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
