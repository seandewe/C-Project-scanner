# Distributed Word Indexing System

## Overview

This project consists of a distributed system with the following C# console applications:

- **ScannerA**: Reads `.txt` files from a directory, indexes word counts, and sends data to Master via named pipe `agent1`.
- **ScannerB**: Similar to ScannerA but communicates through `agent2`.
- **MasterApp**: Collects word index data from both scanners via named pipes, aggregates it, and displays the results.

## Features

- Multi-threaded architecture: separate threads for file reading and communication.
- Uses `NamedPipeServerStream` and `NamedPipeClientStream` for IPC.
- Processor affinity is configured for each process:
  - ScannerA → Core 1
  - ScannerB → Core 2
  - Master → Core 0

## How It Works

1. **Start MasterApp**:
   - Listens for connections on `agent1` and `agent2` pipes.
   - Waits for input from both agents.
2. **Start ScannerA and ScannerB**:
   - Each asks for a directory path.
   - Scans all `.txt` files and counts words.
   - Sends serialized JSON list of word counts to Master.
3. **MasterApp**:
   - Receives data, deserializes it, aggregates it.
   - Prints the full combined word index to the console.

## Build & Run

1. Open each project in separate terminal/VS Code window.
2. Build all projects with `dotnet build`.
3. Run in this order:
   - `dotnet run --project MasterApp`
   - `dotnet run --project ScannerA`
   - `dotnet run --project ScannerB`


