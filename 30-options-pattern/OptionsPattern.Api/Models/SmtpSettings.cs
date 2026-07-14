using System.ComponentModel.DataAnnotations;

namespace OptionsPattern.Api.Models;

/// <summary>
/// SMTP Settings model with DataAnnotations validation.
/// </summary>
public class SmtpSettings
{
    public const string SectionName = "SmtpSettings";

    [Required(ErrorMessage = "SMTP Server is required.")]
    public string Server { get; set; } = string.Empty;

    [Range(1, 65535, ErrorMessage = "Port must be between 1 and 65535.")]
    public int Port { get; set; }

    [Required(ErrorMessage = "Sender Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid Email Address format.")]
    public string SenderEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Sender Name is required.")]
    public string SenderName { get; set; } = string.Empty;

    public bool EnableSsl { get; set; }
}
