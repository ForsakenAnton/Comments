namespace Service.Contracts;

public interface IHtmlSanitizerService
{
    Task<string> SanitizeTextAsync(string text);
}
