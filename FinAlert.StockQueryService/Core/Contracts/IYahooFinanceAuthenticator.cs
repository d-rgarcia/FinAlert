using System.Net;

namespace FinAlert.StockQueryService.Core.Contracts;

public interface IYahooFinanceAuthenticator
{
    public Task EnsureAuthenticationAsync();
    public Cookie GetAuthCookie();
    public string GetCrumb();
}