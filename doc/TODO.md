# Client Desktop TODO (v1) — 2024-07-20
 
Legend:
 - Priority P0 (critical/now), P1 (high/next), P2 (medium/later)
 - [ ] open, [x] done
 
## Recently Completed
 - [x] Initial WinForms UI scaffolding for main window (P0)
 - [x] Basic client authentication flow (username/password login) (P0)
 - [x] Local configuration loading/saving (e.g., API endpoint, user preferences) (P0)
 - [x] Rewrote fully README.md for the client (P0)
 - [x] Added GitHub badges to README.md (P1)
 
## In Progress
 - [ ] Test online clients with max using Radmin VPN (P2)
 - [x] Integrate with the BLUE16 Web API for fetching available Roblox versions (P2)
 - [ ] Implement secure storage for user session tokens locally (e.g., using .NET's Data Protection API) (P2)
 - [x] Design and implement the main "Launch Game" functionality (selecting version, executing game) (P2)
 
## P0 — Next Up (must-have)
 - [ ] **Client Auto-Update Mechanism:** Implement a robust way for the client to detect, download, and apply updates.
 - [x] **Robust Error Handling:** Display user-friendly error messages for common issues (network, API failures, invalid credentials, game launch errors).
 - [ ] **System Tray Integration:** Minimize to tray, show notifications from system tray icon.
 - [x] **Version Selection UI:** Allow users to browse and select different Roblox versions within the client.
 - [ ] **Security Review:** Ensure local credential storage is robust and API communication is secure.
 
## P1 — Backlog (should-have)
 - [ ] **Game Launch Customization:** Options for custom game executables paths, command-line arguments.
 - [x] **Activity Indicators:** Show loading/processing states for network requests and game launches.
 - [x] **Basic Settings Page:** Allow users to change API endpoint (for dev/testing), game install path, logging level.
 - [ ] **Notification System:** In-app pop-ups or toast notifications for important events (update available, game launched, error).
 - [ ] **Improved Logging:** Implement structured logging to a local file for easier debugging.
 - [ ] **"Forgot Password" flow:** Initiate via the client, redirecting to web.
 
## P2 — Nice to Have (could-have)
 - [x] **Theming/Customization:** Allow users to choose basic UI themes or accent colors.
 - [x] **Offline Mode (Limited):** Allow launching previously downloaded versions if offline.
 - [ ] **"Remember Me" Functionality:** Securely persist login for extended periods.
 - [x] **Pre-Launch Checks:** Verify game files, dependencies, or required permissions.
 - [x] **Context Menus:** Right-click options for various UI elements.
 - [x] **Accessibility Improvements:** Basic keyboard navigation, screen reader compatibility.
 
## QA / Verification Checklist
 - [ ] Client installation and uninstallation are clean.
 - [ ] Authentication (login, logout, token refresh) works as expected.
 - [x] Game launching (for various versions) is successful.
 - [ ] All API interactions (fetching versions, user data) handle success/failure gracefully.
 - [x] UI remains responsive during network operations and heavy tasks.
 - [ ] Application handles unexpected closures (crashes, forced shutdowns) without data corruption.
 - [ ] All external links open correctly in the default web browser.
 - [ ] Basic window resize and minimize/maximize functionality is stable.
 
## Tech Debt / Maintenance
 - [ ] **Refactor API Client:** Abstract API calls into a dedicated service layer.
 - [ ] **Unit Tests:** Add unit tests for core logic (e.g., API client, config management).
 - [ ] **UI/Integration Tests:** Implement basic UI or integration tests for key user flows.
 - [ ] **Code Documentation:** Add XML comments for public methods and classes.
 - [ ] **Dependency Management:** Review and update NuGet packages.
 
## Ideas (future)
 - [x] **Mod/Plugin Support:** A simple system for managing game modifications.
 - [ ] **Multiple User Profiles:** Support for switching between different BLUE16 accounts.
 - [ ] **Game Library View:** A more visually appealing way to manage local game installations.
 - [x] **Performance Monitoring:** In-app display of client resource usage.
 - [ ] **Multi-platform support:** Explore MAUI or Avalonia UI for cross-platform clients.