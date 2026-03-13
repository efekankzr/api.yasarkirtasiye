using System;

namespace api.yasarkirtasiye.Domain.Entities;

public class SiteSetting : BaseEntity
{
    public string? Description { get; set; }
    public string? Phone { get; set; }
    public string? WhatsAppNumber { get; set; }  // Ayrı WhatsApp numarası (yoksa Phone kullanılır)
    public string? WhatsAppTemplate { get; set; } // WhatsApp önizleme mesaj şablonu
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? LogoUrl { get; set; }          // Admin panelinden yüklenen logo
    public string? FacebookUrl { get; set; }
    public string? TwitterUrl { get; set; }
    public string? InstagramUrl { get; set; }
}
