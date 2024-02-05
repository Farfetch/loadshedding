.PHONY: sample sample-clean

sample:
	@docker compose -f docker-compose.sample.yaml up --build -d

sample-clean:
	@docker compose -f docker-compose.sample.yaml down
