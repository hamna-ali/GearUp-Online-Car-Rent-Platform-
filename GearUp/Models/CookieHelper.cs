using System.Text.Json;

public static class CookieHelper
{
    public static void SetCookie(HttpResponse response, string key, object value)
    {
        var options = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(7),
            HttpOnly = true
        };
        string json = JsonSerializer.Serialize(value);
        response.Cookies.Append(key, json, options);
    }

    public static T? GetCookie<T>(HttpRequest request, string key)
    {
        if (request.Cookies.TryGetValue(key, out string? cookie))
        {
            return JsonSerializer.Deserialize<T>(cookie);
        }
        return default;
    }
}
