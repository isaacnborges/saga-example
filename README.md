# saga-example

This project shows how to implement the [SAGA pattern](https://microservices.io/patterns/data/saga.html) using [MassTransit](https://masstransit-project.com/)


## Dependencies 

- [Docker](https://docs.docker.com/get-docker/)

## How to run

After cloning this repository go into the directory and run `docker-compose up -d`.

## Debug 

- [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [Visual Studio](https://docs.microsoft.com/en-us/visualstudio/containers/edit-and-refresh?view=vs-2019)
- [Visual Studio Code](https://code.visualstudio.com/docs/containers/debug-netcore)

## Informations

This solution contains some APIs and some workers (consumers/publishers) that communicate with each other using synchronous communication with [HTTP request](https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods) or asynchronous messaging with [RabbitMQ](https://www.rabbitmq.com/) to implementing the SAGA pattern.

### Projects
- Cart.Api: API responsible for management the carts;
- Order.Api: API responsible for management the orders;
- Order.Domain: Project responsible for orchestrating the saga;
- Order.Worker: Project responsible for publish the commands and consume the events
- OrderVendor.Worker: Project that simulate an integration with a partner;
- Payment.Api: API responsible for management the payments;
- Payment.Domain: Project responsible to maintain payment contracts (commands/events);
- Payment.Worker: Project that simulate authorize and confirm payments;
- Saga.Core: Cross project to facilitate commons implementations between the projects;

### URLs
- RabbitMQ: http://localhost:15672/ - guest/guest
- Jaeger: http://localhost:16686/
- Order.Api: http://localhost:4000/swagger/
- Cart.Api: http://localhost:4001/swagger/
- Payment.Api: http://localhost:4002/swagger/