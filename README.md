# ChroniLog 🕒📜
ChroniLog is a .NET **asynchronous logging** modular library,
designed to be extensible, efficient, and easy to integrate. It allows you to save logs in different targets using **(*flush-target*)** packages, starting with **PostgreSQL** support.

<img src="https://raw.githubusercontent.com/sicilo/ChroniLog/master/ChroniLog.Core/logo.png" alt="ChroniLog Logo" width="150"/>

## 🚀 Related projects

- **ChroniLog.Core**  
  Library core. includes a base logging system, its asynchronous worker, and its provider.

- **ChroniLog.Flusher.PostgreSql**  
  *flush-target* implementation to PostgreSQL.

## ✨ Characteristics

- Structured Logging based on `ILogger`.
- Asynchronous background processing using `ServiceWorker`.
- Extensible: you can create your own *flush-targets* providers.
- Compatible with .NET 8+.

## 📦 Usage

To use the core and create your own implementation, follow the documentation below:

- [Core](ChroniLog.Core/README.md)

To use the PostgreSQL *flush-target*, follow the documentation below:

- [PostgreSQL Flush](ChroniLog.Flusher.PostgreSql)
