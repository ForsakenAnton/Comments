
using Entities.ValidationModels;
using Service.Contracts;

namespace Service;

internal sealed class HtmlSanitizerService : IHtmlSanitizerService
{
    public Task<string> SanitizeTextAsync(string text)
    {
        HtmlTextValidator htmlTextValidator = new HtmlTextValidator();
        string sanitizedText = htmlTextValidator.SanitizeText(text);

        return Task.FromResult(sanitizedText);
    }
}
