.PHONY: sample sample-clean

sample:
	@docker compose -f docker-compose.sample.yaml up --build -d

sample-clean:
	@docker compose -f docker-compose.sample.yaml down

benchmark:
	dotnet run -c Release --project ./tests/benchmark/Farfetch.LoadShedding.BenchmarkTests/Farfetch.LoadShedding.BenchmarkTests.csproj
