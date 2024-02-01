---
sidebar_position: 4
---

# Samples

We know that working code is a valuable learning tool for many, so here you can find a list of samples built to demonstrate LoadShedding capabilities.

## Web API

This is a simple sample that shows how LoadShedding is implemented.

You can find the code here: [/samples/Samples.WebApi](https://github.com/Farfetch/loadshedding/tree/master/samples/Samples.WebApi)

To run this sample with docker environment please follow the following steps:

### 1. Run docker compose command

Inside the _loadshedding_ project directory, run the following command:

```bash
docker compose -f docker-compose.sample.yaml up --build -d
```

Alternatively, you can achieve the same result using the following make command:

```bash
make sample
```

### 2. Loadshedding metrics with Grafana

To view the Grafana dashboard just navigate to http://localhost:3000 and login with the default admin credentials (username: admin, password: admin).

After that, you can access the dashboard here: http://localhost:3000/d/http_loadshedding/http-loadshedding?orgId=1&refresh=1m
