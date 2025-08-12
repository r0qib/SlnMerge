# SlnMerge

SlnMerge is a .NET command-line tool for merging multiple Visual Studio solution (.sln) files into a single solution. It helps consolidate projects and global sections from different solutions, making it easier to manage large codebases.

## Setup

1. Ensure [.NET 9.0 SDK](https://dotnet.microsoft.com/download) is installed.
2. Build the project:
   ```sh
   dotnet build
   ```

## Usage

To merge two solution files:

```sh
dotnet run --project SlnMerge/SlnMerge.csproj -- SlnMerge/source.sln SlnMerge/target.sln
```

This will merge `source.sln` into `target.sln`. The merged solution will be saved as `target.sln`.