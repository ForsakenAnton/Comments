// Ignore Spelling: Validator

using Ganss.Xss;

namespace Entities.ValidationModels;

public class HtmlTextValidator
{
    private readonly HtmlSanitizer _sanitizer;

    public HtmlTextValidator()
    {
        _sanitizer = new HtmlSanitizer();
        _sanitizer.AllowedTags.Clear();
        _sanitizer.AllowedTags.Add("a");
        _sanitizer.AllowedTags.Add("code");
        _sanitizer.AllowedTags.Add("i");
        _sanitizer.AllowedTags.Add("strong");

        _sanitizer.AllowedAttributes.Add("href");
        _sanitizer.AllowedAttributes.Add("title");
    }

    public string SanitizeText(string input)
    {
        return _sanitizer.Sanitize(input);
    }
}
