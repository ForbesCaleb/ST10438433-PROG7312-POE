namespace SmartX.Client.Services;

public class AuthState
{
    public bool IsAuthenticated { get; private set; }
    public string UserName { get; private set; } = string.Empty;
    public bool IsGuest { get; private set; }
    public string? IdToken { get; private set; }

    public event Action? OnChange;

    public void SignIn(string userName, string? idToken = null, bool asGuest = false)
    {
        IsAuthenticated = true;
        UserName = userName;
        IdToken = idToken;
        IsGuest = asGuest;
        OnChange?.Invoke();
    }

    public void SignOut()
    {
        IsAuthenticated = false;
        UserName = string.Empty;
        IdToken = null;
        IsGuest = false;
        OnChange?.Invoke();
    }
}