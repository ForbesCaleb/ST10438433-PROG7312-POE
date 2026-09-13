using Microsoft.AspNetCore.SignalR;

namespace SmartX.Api;

// This hub is the transport for your Dynamic Engagement Feature — it pushes
// live telemetry to every connected browser instead of the UI polling for updates.
public class TelemetryHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("Connected", "Connected to Smart-X telemetry hub.");
        await base.OnConnectedAsync();
    }
}