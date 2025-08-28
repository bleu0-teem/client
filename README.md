# BLUE16 Desktop Client

<p align="center">
  <img src="https://img.shields.io/github/stars/bleu0-teem/client?style=for-the-badge&logo=github&label=Stars" alt="GitHub Stars"/>
  &nbsp;
  <img src="https://img.shields.io/github/forks/bleu0-teem/client?style=for-the-badge&logo=github&label=Forks" alt="GitHub Forks"/>
</p>

A lightweight Windows desktop client for the BLUE16 project, designed for integrating with the BLUE16 launcher. This client offers a native interface for your ROBLOX revival experience, focusing on performance and ease of use.

---

## Features

- **Efficient and Lightweight:** Minimal resource usage for fast performance.
- **Native Windows Interface:** Built with WinForms for a familiar desktop experience.
- **Launcher Integration:** Works seamlessly with the BLUE16 launcher.
- **Simple UI:** Easy-to-use, straightforward navigation.

---

## Technology Stack

- **Framework:** .NET 8
- **UI Toolkit:** Windows Forms (WinForms)
- **Language:** C#
- **Platform:** Windows

---

## Requirements

To build and run the BLUE16 Desktop Client:

- **Windows 10 or newer**
- **.NET 8 SDK** ([Download here](https://dotnet.microsoft.com/download/dotnet/8.0))
- **Git**

---

## Getting Started

Follow these steps to build and run the client:

### Build from Source

1. **Clone the repository**
    ```bash
    git clone https://github.com/bleu0-teem/client.git
    cd client
    ```

2. **Restore dependencies**
    ```bash
    dotnet restore
    ```

3. **Build the project**
    ```bash
    dotnet build -c Release
    ```
    The executable will be in `BLUE16Client/bin/Release/net8.0-windows/`.

### Run the Client

You can start the application in several ways:

- **From Visual Studio**
    1. Open `BLUE16Client.sln` in Visual Studio.
    2. Set `BLUE16Client` as the startup project.
    3. Press `F5` or click "Start Debugging".

- **Using the .NET CLI**
    ```bash
    dotnet run --project BLUE16Client/BLUE16Client.csproj
    ```

- **Running the Executable**
    Go to `BLUE16Client/bin/Release/net8.0-windows/` and run `BLUE16Client.exe`.

---

## Contributing

Contributions are welcome! If you'd like to help improve the BLUE16 Desktop Client, check out the [`CONTRIBUTING.md`](CONTRIBUTING.md) guide for details.

---

## FAQ

**Why WinForms for the UI?**  
WinForms provides direct Windows integration, ensuring a familiar and lightweight desktop experience.

**What is the BLUE16 launcher?**  
The launcher is part of the BLUE16 project, a ROBLOX revival platform supporting historic Roblox versions. This client offers a desktop interface for it.

**How can I report issues or suggest features?**  
Open an issue on our GitHub repository.

---

## License

This project is licensed under the [MIT License](LICENSE).

Copyright © [bleu0 teem omg](https://github.com/bleu0-teem)

---

### Support the Project

If you find BLUE16 useful, please star the repository to help support the project!