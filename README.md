# 💻 BLUE16 Desktop Client

<p align="center">
  <img src="https://img.shields.io/github/stars/bleu0-teem/client?style=for-the-badge&logo=github&label=Stars" alt="GitHub Stars"/>
  &nbsp;
  <img src="https://img.shields.io/github/forks/bleu0-teem/client?style=for-the-badge&logo=github&label=Forks" alt="GitHub Forks"/>
</p>

A dedicated, lightweight Windows client designed to seamlessly integrate with the **BLUE16 launcher**. Experience an efficient and native desktop interface for your ROBLOX revival journey, focusing on core functionality and a smooth user experience.

---

## ✨ Features

*   **Lightweight & Efficient:** Built for minimal resource consumption and fast execution, ensuring a responsive client.
*   **Native Windows Feel:** Developed using WinForms to provide a familiar and direct desktop experience for Windows users.
*   **Seamless Launcher Integration:** Designed to work hand-in-hand with the BLUE16 launcher for a unified experience.
*   **Intuitive Interface:** A straightforward and easy-to-navigate user interface.

---

## 🛠️ Tech Stack

*   **Framework:** .NET 8
*   **UI Toolkit:** Windows Forms (WinForms)
*   **Language:** C#
*   **Platform:** Windows

---

## 🚀 Requirements

To build and run the BLUE16 Desktop Client from source, you will need:

*   **Operating System:** Windows 10 or newer
*   **SDK:** [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
*   **Version Control:** Git

---

## 🏁 Getting Started

Follow these steps to get the BLUE16 Desktop Client up and running.

### Building from Source

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/bleu0-teem/client.git # Adjust if your repo name is different
    cd client
    ```

2.  **Restore dependencies:**
    Navigate to the project directory (usually where your `.sln` file is located, or directly to the `BLUE16Client` project folder) and run:
    ```bash
    dotnet restore
    ```

3.  **Build the project:**
    ```bash
    dotnet build -c Release
    ```
    The executable will be located in `BLUE16Client/bin/Release/net8.0-windows/`.

### Running the Client

You have a few options to run the application:

*   **From Visual Studio:**
    1.  Open the `BLUE16Client.sln` solution file in Visual Studio.
    2.  Set `BLUE16Client` as the startup project.
    3.  Press `F5` or click the "Start Debugging" button.

*   **Using the .NET CLI:**
    ```bash
    dotnet run --project BLUE16Client/BLUE16Client.csproj
    ```

*   **Running the Built Executable:**
    Navigate to the `BLUE16Client/bin/Release/net8.0-windows/` directory (or the path where you built it) and run `BLUE16Client.exe`.

---

## 👋 Contributing

We welcome contributions from the community! If you're interested in helping improve the BLUE16 Desktop Client, please refer to our [`CONTRIBUTING.md`](CONTRIBUTING.md) guide for detailed information on how to get involved.

---

## ❓ FAQ

**Q: Why was WinForms chosen for the UI?**
A: WinForms was chosen for its direct integration with Windows, providing a native and familiar user experience, and to ensure a lightweight and efficient client.

**Q: What is the BLUE16 launcher?**
A: The BLUE16 launcher is part of a larger ROBLOX revival project aimed at supporting all possible historic Roblox versions. This client provides a desktop interface for it.

**Q: How do I report issues or suggest features?**
A: Please open an issue on our GitHub repository.

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).

Copyright © [bleu0 teem omg](https://github.com/bleu0-teem)

---

### Loved BLUE16 Client? Show your support! ⭐
If you find this project exciting or useful, please consider starring our repository! Your support helps our open-source efforts thrive.