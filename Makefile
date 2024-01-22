.PHONY: build unit-tests integration-tests performance-tests clean sample sample-clean

build:
	@docker compose up --build --no-start

unit-tests: build
	@docker compose up --abort-on-container-exit --exit-code-from unit-tests unit-tests	

integration-tests: build
	@docker compose up --abort-on-container-exit --exit-code-from integration-tests integration-tests	

performance-tests: build
	@docker compose up -d performance-tests-sample
	@docker compose up -d performance-tests-sample-no-limiter

	@docker compose up --abort-on-container-exit --exit-code-from performance-tests performance-tests

clean:
	@docker compose down
	@docker compose -f docker-compose.sample.yaml down
	
sample:
	@docker compose -f docker-compose.sample.yaml up -d

sample-clean:
	@docker compose -f docker-compose.sample.yaml down
