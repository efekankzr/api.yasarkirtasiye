using System;

namespace api.yasarkirtasiye.Application.Features.Settings.DTOs;

public class SiteSettingDto
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public string? Phone { get; set; }
    public string? WhatsAppNumber { get; set; }
    public string? WhatsAppTemplate { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? LogoUrl { get; set; }
    public string? FacebookUrl { get; set; }
    public string? TwitterUrl { get; set; }
    public string? InstagramUrl { get; set; }
}
