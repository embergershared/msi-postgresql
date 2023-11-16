# Access Azure Database for PostgreSQL flexible server with Managed Identity

## Overview

This .NET Console uses an Azure Managed Identity to connect to a database hosted in Azure Database for PostgreSQL flexible server.

It covers a use-case from a customer that:

- uses 3 connection strings in the same application
- to connect to the same database with different permissions:
  - reader
  - schema manager
  - data contributor

The aim was to see if, from the same Azure Platform resource (AKS or App Service), with System-managed identity, it was possible to connect through these 3 different `database` users, but with the same System-managed identity.

Enjoy,

Emmanuel.

## Disclaimer

