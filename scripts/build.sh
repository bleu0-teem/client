#!/usr/bin/env bash

set -euo pipefail

dotnet restore
dotnet build -c Release

echo "Build succeeded"
