# SlnMerge

SlnMerge is a .NET command-line tool for merging multiple Visual Studio solution (.sln) files into a single solution. It helps consolidate projects and global sections from different solutions, making it easier to manage large codebases.

## Setup

1. Ensure [.NET 9.0 SDK](https://dotnet.microsoft.com/download) is installed.
2. Build the project:
   ```sh
   dotnet build
   ```

## Usage

SlnMerge merges two Visual Studio solution files: a source (typically the main branch) and a target (typically a feature/dev/test branch). The merged result is saved to an output file.

You can run SlnMerge in two ways:

### 1. Using Environment Variables

Set the following environment variables before running:

- `SOURCE_SOLUTION` (default: `source.txt`) — main branch solution
- `TARGET_SOLUTION` (default: `target.txt`) — feature/dev/test branch solution
- `OUTPUT_SOLUTION` (default: `output.txt`) — merged output file

**Sample command:**
```sh
set SOURCE_SOLUTION=main.sln
set TARGET_SOLUTION=feature.sln
set OUTPUT_SOLUTION=merged.sln
dotnet run --project SlnMerge/SlnMerge.csproj
```

### 2. Using Command-Line Arguments

Pass solution file paths as arguments:

- `--source=path` — main branch solution
- `--target=path` — feature/dev/test branch solution
- `--output=path` — merged output file

**Sample command:**
```sh
dotnet run --project SlnMerge/SlnMerge.csproj -- --source=main.sln --target=feature.sln --output=merged.sln
```

If no arguments or environment variables are provided, defaults are used: `source.txt`, `target.txt`, and `output.txt`.