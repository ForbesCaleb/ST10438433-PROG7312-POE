# Smart-X IoT Mesh Gateway — Part 1
### Sensor Data Ingestion and Validation Gateway

**Module:** PROG7312 / AAPD7112 — Advanced Application Development
**Student Number:** ST10438433

## Architecture

- **SmartX.Shared** — class library containing all shared models and logic used by both the API
  and the client:
  - `TelemetryPacket<T>` — generic wrapper for float/int/bool sensor readings (Generics)
  - `PowerReading` — overloaded `+`, `-`, `>`, `<` operators for aggregating/comparing smart-meter loads (Operator Overloading)
  - `DeploymentNode` + `DeploymentValidator` — recursive validation of nested device deployment trees (Recursion)
  - `TelemetryBatchProcessor` — converts jagged-array telemetry batches into optimised `List<T>` collections
  - `AnomalyChecker` — generic threshold check used across telemetry types
- **SmartX.Api** — ASP.NET Core Minimal API. Hosts the ingestion endpoints, a SignalR hub for live
  telemetry, and a background service that seeds mock sensor data automatically.
- **SmartX.Client** — Blazor WebAssembly (standalone) frontend. Landing page with the three
  architectural pillars (only "Sensor Data Ingestion & Telemetry" is enabled in this part) and the
  live ingestion dashboard.

## Prerequisites

- Visual Studio 2026 with the **ASP.NET and web development** workload installed
- .NET 10 SDK

## How to restore and run

1. Open `SmartX.sln` in Visual Studio 2026.
2. Let NuGet restore automatically (or run `dotnet restore` from the solution folder).
3. **First run only:** start `SmartX.Api` by itself (right-click → Debug → Start New Instance) and
   note the HTTPS port printed in the console, e.g. `https://localhost:XXXX`.
4. If needed, update that port in two places in `SmartX.Client` so it matches:
   - `Program.cs` → the `HttpClient` `BaseAddress`
   - `Pages/Ingestion.razor` → the `HubConnectionBuilder().WithUrl(...)` call
5. Right-click the **Solution** → **Configure Startup Projects** → *Multiple startup projects* →
   set both `SmartX.Api` and `SmartX.Client` to **Start** (API listed above Client) → OK.
6. Press **F5**. The API immediately starts seeding mock telemetry every 2 seconds; the Blazor app
   opens separately and connects to the API automatically.
7. Navigate to **Sensor Data Ingestion & Telemetry** on the landing page to view the live dashboard.

## Demo walkthrough

- Three-pillar landing page, with two pillars visibly disabled pending future PoE parts.
- Register a new sensor (MAC address, zone/location, category) with an optional file attachment.
- Live sensor tiles update in real time via SignalR; a tile flashes red when the simulator produces
  an anomalous reading, backed by the anomaly-alerting engagement strategy researched in Task 1.
- "Run Demos" button on the Ingestion page calls the API to show the recursion, operator
  overloading, and jagged-array/`List<T>` results running live.

## Known limitations

- Telemetry and sensor data are stored in-memory and reset on API restart — acceptable for this
  simulated assessment environment.
- "Real-Time Command Stream & History" and "Network Topology & Mesh Routing" pillars are
  intentionally disabled; scoped for Part 2 and the final PoE respectively.
