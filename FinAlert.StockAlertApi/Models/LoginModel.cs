using System.ComponentModel.DataAnnotations;

namespace FinAlert.StockAlertApi.Models;

public class LoginModel
{
    [Required]
    [EmailAddress]
    public string Email { get; internal set; } = string.Empty;
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; internal set; } = string.Empty;
}