using Nop.Core.Configuration;

namespace Nop.Core.Domain.Translation;

/// <summary>
/// Translation settings
/// </summary>
public partial class TranslationSettings : ISettings
{
    /// <summary>
    /// Is pre translate allowed?
    /// </summary>
    public bool AllowPreTranslate { get; set; }

    /// <summary>
    /// Translate from language
    /// </summary>
    public int TranslateFromLanguageId { get; set; }

    /// <summary>
    /// A list of languages which is not allowed to pre translate
    /// </summary>
    public List<int> NotTranslateLanguages { get; set; }

    /// <summary>
    /// The Google Translate API key
    /// </summary>
    public string GoogleApiKey { get; set; }

    /// <summary>
    /// The DeepL Auth key
    /// </summary>
    public string DeepLAuthKey { get; set; }

    /// <summary>
    /// The translation service type id
    /// </summary>
    public int TranslationServiceId { get; set; }
}