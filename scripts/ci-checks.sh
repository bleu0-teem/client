#!/usr/bin/env bash
set -euo pipefail

dotnet restore
dotnet build -c Release --no-restore
