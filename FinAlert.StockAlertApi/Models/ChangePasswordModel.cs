using System.ComponentModel.DataAnnotations;

namespace FinAlert.StockAlertApi.Models;

public class ChangePasswordModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; } = string.Empty;
    [Required]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;
    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword))]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}