using System.Net.Http.Json;

namespace SmartX.Client.Services;

public class FirebaseAuthService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public FirebaseAuthService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["Firebase:ApiKey"]
            ?? throw new InvalidOperationException("Firebase API key not found in appsettings.json.");
    }

    public async Task<FirebaseAuthResult?> SignInAsync(string email, string password)
    {
        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_apiKey}";
        var response = await _http.PostAsJsonAsync(url, new { email, password, returnSecureToken = true });

        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<FirebaseAuthResult>()
            : null;
    }
}

public record FirebaseAuthResult(string IdToken, string Email, string RefreshToken, string ExpiresIn, string LocalId);