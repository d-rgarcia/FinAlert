using System.Diagnostics;
using System.Net;
using FinAlert.StockQueryService.Core.Contracts;
using Microsoft.Extensions.Logging;

namespace FinAlert.StockQueryService.Services;

internal class YahooFinanceAuthenticator : IYahooFinanceAuthenticator
{
    public const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)";

    private readonly ILogger<YahooFinanceAuthenticator> _logger;
    private readonly HttpClient _httpClient;
    private string _crumb;
    private Cookie _authCookie;

    public YahooFinanceAuthenticator(HttpClient httpClient, ILogger<YahooFinanceAuthenticator> logger)
    {
        _httpClient = httpClient;
        _crumb = string.Empty;
        _authCookie = new Cookie();
        _logger = logger;
    }

    public async Task EnsureAuthenticationAsync()
    {
        await SetAuthenticationCookie();

        await SetAuthenticationCrumb();
    }

    public void ClearAuthentication()
    {
        _authCookie = null!;
        _crumb = null!;
    }

    public Cookie GetAuthCookie() => _authCookie;

    public string GetCrumb() => _crumb;

    #region private Methods

    private async Task SetAuthenticationCrumb()
    {
        if (!string.IsNullOrEmpty(_crumb))
            return;

        var stopwatch = Stopwatch.StartNew();
        
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://query1.finance.yahoo.com/v1/test/getcrumb");
        request.Headers.UserAgent.ParseAdd(USER_AGENT);
        request.Headers.Add("Cookie", $"{_authCookie.Name}={_authCookie.Value}");
        HttpResponseMessage response = await _httpClient.SendAsync(request);
        _crumb = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrEmpty(_crumb))
            throw new Exception("Failed to retrieve Yahoo crumb.");

        stopwatch.Stop();

        _logger.LogInformation($"Yahoo crumb retrived in {stopwatch.ElapsedMilliseconds} ms.");
    }

    private async Task SetAuthenticationCookie()
    {
        if (_authCookie is not null)
            return;

        var stopwatch = Stopwatch.StartNew();

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://fc.yahoo.com");
        request.Headers.UserAgent.ParseAdd(USER_AGENT);
        HttpResponseMessage response = await _httpClient.SendAsync(request);
        var cookieHeader = response.Headers.GetValues("Set-Cookie").FirstOrDefault();
        if (cookieHeader == null || !cookieHeader.Contains("A3"))
            throw new Exception("Failed to obtain Yahoo auth cookie.");

        _authCookie = new Cookie("A3", cookieHeader.Split(';')[0].Split("A3=")[1], "/", ".yahoo.com");

        stopwatch.Stop();

        _logger.LogInformation($"Yahoo cookie retrived in {stopwatch.ElapsedMilliseconds} ms.");
    }

    #endregion
}