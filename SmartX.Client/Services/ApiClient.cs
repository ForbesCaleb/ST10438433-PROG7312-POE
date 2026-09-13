using System.Net.Http.Json;
using SmartX.Shared.Models;

namespace SmartX.Client.Services;

public class ApiClient
{
    private readonly HttpClient _http;
    public ApiClient(HttpClient http) => _http = http;

    public async Task<bool> RegisterSensorAsync(SensorRegistration registration)
    {
        var response = await _http.PostAsJsonAsync("api/sensors", registration);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<SensorRegistration>?> GetSensorsAsync()
        => await _http.GetFromJsonAsync<List<SensorRegistration>>("api/sensors");

    public async Task<bool> UploadAttachmentAsync(string mac, IReadOnlyList<Microsoft.AspNetCore.Components.Forms.IBrowserFile> files)
    {
        if (files.Count == 0) return false;
        var file = files[0];

        using var content = new MultipartFormDataContent();
        using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
        using var streamContent = new StreamContent(stream);
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
        content.Add(streamContent, "file", file.Name);

        var response = await _http.PostAsync($"api/sensors/{mac}/attachment", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<DeploymentValidationResult?> GetDeploymentValidationAsync()
        => await _http.GetFromJsonAsync<DeploymentValidationResult>("api/deployment/validate");

    public async Task<List<SoilBatchSummary>?> GetSoilHistoryAsync()
        => await _http.GetFromJsonAsync<List<SoilBatchSummary>>("api/telemetry/history/soil-batch");

    public async Task<PowerAggregateResult?> GetPowerAggregateAsync()
        => await _http.GetFromJsonAsync<PowerAggregateResult>("api/power/aggregate");
}

public record DeploymentValidationResult(bool IsValid, string? FirstInvalidPath);
public record SoilBatchSummary(string Sensor, List<double> Readings, double Average);
public record PowerAggregateResult(string Meter1, string Meter2, string Aggregate, double Delta, bool Meter1IsHigher);