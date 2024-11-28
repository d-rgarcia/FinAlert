using System.Net;
using FinAlert.StockQueryService.Core.Contracts;

namespace FinAlert.StockQueryService.Services;

internal class YahooFinanceAuthenticator : IYahooFinanceAuthenticator
{
    public const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)";

    private readonly HttpClient _httpClient;
    private string _crumb;
    private Cookie _authCookie;

    public YahooFinanceAuthenticator(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _crumb = string.Empty;
        _authCookie = new Cookie();
    }

    public async Task EnsureAuthenticationAsync()
    {
        if (!string.IsNullOrEmpty(_crumb) && _authCookie != null)
            return;

        var request = new HttpRequestMessage(HttpMethod.Get, "https://fc.yahoo.com");
        request.Headers.UserAgent.ParseAdd(USER_AGENT);
        var response = await _httpClient.SendAsync(request);

        var cookieHeader = response.Headers.GetValues("Set-Cookie").FirstOrDefault();
        if (cookieHeader == null || !cookieHeader.Contains("A3"))
            throw new Exception("Failed to obtain Yahoo auth cookie.");

        _authCookie = new Cookie("A3", cookieHeader.Split(';')[0].Split("A3=")[1], "/", ".yahoo.com");

        request = new HttpRequestMessage(HttpMethod.Get, "https://query1.finance.yahoo.com/v1/test/getcrumb");
        request.Headers.UserAgent.ParseAdd(USER_AGENT);
        request.Headers.Add("Cookie", $"{_authCookie.Name}={_authCookie.Value}");
        response = await _httpClient.SendAsync(request);
        _crumb = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrEmpty(_crumb))
            throw new Exception("Failed to retrieve Yahoo crumb.");
    }

    public Cookie GetAuthCookie() => _authCookie;

    public string GetCrumb() => _crumb;
}