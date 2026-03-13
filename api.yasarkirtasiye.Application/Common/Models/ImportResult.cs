namespace api.yasarkirtasiye.Application.Common.Models;

public class ImportResult
{
    public int SuccessCount { get; set; }
    public List<string> Errors { get; set; } = new();
}
