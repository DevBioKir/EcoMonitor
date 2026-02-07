using System.Text;
using EcoMonitor.AdminPanel.Infrastucture;
using EcoMonitor.AdminPanel.Infrastucture.TokenStorage;
using Microsoft.JSInterop;

namespace EcoMonitor.AdminPanel.Components.Pages.currentUserUI;

public class CurrentUserService
{
    private readonly ITokenStore _tokenStore;
    private readonly IJSRuntime _js;
    private readonly UISync _sync;
    
    public event Action? OnChange;
    
    public CurrentUserService(ITokenStore tokenStore, IJSRuntime js, UISync sync)
    {
        _tokenStore = tokenStore;
        _js = js;
        _sync = sync;

        _tokenStore.OnTokensLoaded += async () =>
        {
            Console.WriteLine("CurrentUserService: tokens loaded, loading user..."); 
            await LoadUserAsync();
        };
    }
    
    public string? UserName { get; private set; }
    
    public async Task LoadUserAsync()
    {
        Console.WriteLine("=== LoadUserAsync START ===");
        
        if (_tokenStore.HasAccessToken())
        {
            var token = _tokenStore.AccessToken;
            Console.WriteLine("TOKEN RAW: " + token);
            UserName = ParseUserNameFromToken(token);
            Console.WriteLine("PARSED USERNAME: " + UserName);
        }
        else
        {
            Console.WriteLine("NO ACCESS TOKEN FOUND");
            UserName = "Гость";
        }

        NotifyStateChanged();
        Console.WriteLine("=== LoadUserAsync END ===");
        
        await Task.CompletedTask;
    }

    private void NotifyStateChanged()
    {
        Console.WriteLine("=== NotifyStateChanged ===");
        if (_sync.Context == null)
        {
            Console.WriteLine("UISync.Context is null → skipping UI update"); 
            return;
        }
        
        _sync.Context.Post(_ => 
            OnChange?.Invoke(), 
            null);
        // _ = _js.InvokeAsync<object>("", null).AsTask().ContinueWith(_ =>
        // {
        //     OnChange?.Invoke();
        // });
    }
    
    public void Clear()
    {
        Console.WriteLine("=== CLEAR USER ===");
        UserName = "Гость";
        //NotifyStateChanged();
    }

    private string ParseUserNameFromToken(string token)
    {
        Console.WriteLine("=== ParseUserNameFromToken START ===");
        var parts = token.Split('.');
        
        Console.WriteLine("TOKEN: " + parts);
        
        if (parts.Length != 3)
        {
            Console.WriteLine("INVALID TOKEN FORMAT"); 
            return "Гость";
        }
        Console.WriteLine("TOKEN PARTS COUNT: " + parts.Length);
        
        var payloadBase64 = parts[1];
        Console.WriteLine("PAYLOAD BASE64URL: " + payloadBase64);
        
        var jsonBytes = Base64UrlDecode(payloadBase64); 
        var json = Encoding.UTF8.GetString(jsonBytes);
        Console.WriteLine("PAYLOAD JSON: " + json);
        
        var obj = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);

        if (obj == null)
        {
            Console.WriteLine("DESERIALIZED PAYLOAD IS NULL");
            return "Гость"; 
        }

        if (obj.TryGetValue("name", out var name))
        {
            Console.WriteLine("FOUND CLAIM 'name': " + name); 
            return name.ToString();
        }
        
        if (obj.TryGetValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", out var name2)) 
            return name2.ToString();

        if (obj.TryGetValue("email", out var email))
        {
            Console.WriteLine("FOUND CLAIM 'email': " + email); 
            return email.ToString();
        }
        
        if (obj.TryGetValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", out var email2)) 
            return email2.ToString();
        
        Console.WriteLine("NO NAME CLAIM FOUND, RETURNING 'Гость'"); 
        return "Гость";
            
        // return obj.TryGetValue("name", out var name) 
        //     ? name.ToString() : obj.TryGetValue("email", out var email) ? email.ToString() : "Гость";
    }

    private static byte[] Base64UrlDecode(string input)
    {
        string output = input.Replace('-', '+').Replace('_', '/');
        switch (output.Length % 4)
        {
            case 2: output += "=="; 
                break; 
            case 3: output += "="; 
                break;
        } 
        return Convert.FromBase64String(output);
    }
}