using System.Reflection;
using DeepL;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Translation;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Framework.Translation;

/// <summary>
/// Provides methods for translation
/// </summary>
public partial class TranslationService : ITranslationService
{
    #region Fields

    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILogger _logger;
    protected readonly TranslationSettings _translationSettings;

    #endregion

    #region Ctor

    public TranslationService(ILanguageService languageService, ILocalizationService localizationService, ILogger logger, TranslationSettings translationSettings)
    {
        _languageService = languageService;
        _localizationService = localizationService;
        _logger = logger;
        _translationSettings = translationSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Translate text or html
    /// </summary>
    /// <param name="originalLanguage">The language to translate from</param>
    /// <param name="originText">The text or HTML to translate</param>
    /// <param name="targetLanguage">The target language to translate</param>
    /// <param name="isHtml">Indicate is the text to translate should be considered as HTML</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the translated text
    /// </returns>
    protected virtual async Task<string> TranslateAsync(Language originalLanguage, string originText, Language targetLanguage, bool isHtml)
    {
        var result = string.Empty;

        switch ((TranslationServiceType)_translationSettings.TranslationServiceId)
        {
            case TranslationServiceType.GoogleTranslate:
                {
                    var client = Google.Cloud.Translation.V2.TranslationClient.CreateFromApiKey(_translationSettings.GoogleApiKey);

                    var response = isHtml
                        ? await client.TranslateHtmlAsync(originText, targetLanguage.UniqueSeoCode, originalLanguage.UniqueSeoCode)
                        : await client.TranslateTextAsync(originText, targetLanguage.UniqueSeoCode, originalLanguage.UniqueSeoCode);

                    result = response.TranslatedText;
                }
                break;
            case TranslationServiceType.DeepL:
                {
                    var client = new DeepLClient(_translationSettings.DeepLAuthKey);
                    var response = await client.TranslateTextAsync(originText, originalLanguage.UniqueSeoCode, targetLanguage.UniqueSeoCode, new TextTranslateOptions
                    {
                        TagHandling = isHtml ? "html" : null
                    });
                    result = response.Text;
                }
                break;
        }

        return result;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Translate the properties
    /// </summary>
    /// <param name="model">The localized model to translate</param>
    /// <param name="propertiesToTranslate">List of properties which should be translated</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task<Dictionary<string, string>> TranslateAsync<T>(ILocalizedModel<T> model, ITranslationService.PropertyToTranslate[] propertiesToTranslate) where T : ILocalizedLocaleModel
    {
        var result = new Dictionary<string, string>();

        var properties = propertiesToTranslate.Select(p => new KeyValuePair<string, bool>(p.PropertyName, p.IsHtml)).ToDictionary();

        //get model properties to use as original text for translation
        var modelProperties = model.GetType().GetProperties().Where(propertyFilter)
            .ToList();

        //get properties to save translated text
        var localizedProperties = typeof(T).GetProperties().Where(propertyFilter)
            .ToList();

        //get original language
        var originalLanguage = await _languageService.GetLanguageByIdAsync(_translationSettings.TranslateFromLanguageId);
        var position = 0;

        //the loop by locales which should be translated
        foreach (var modelLocale in model.Locales)
        {
            //we ignore the original language and languages which should be ignored deepens on settings
            if (modelLocale.LanguageId == _translationSettings.TranslateFromLanguageId ||
                _translationSettings.NotTranslateLanguages.Contains(modelLocale.LanguageId))
            {
                position += 1;
                continue;
            }

            //get target languages to translation for
            var translateToLanguage = await _languageService.GetLanguageByIdAsync(modelLocale.LanguageId);

            //the loop by properties which should be translated
            foreach (var prop in localizedProperties)
            {
                //get the current value of property
                var translated = prop.GetValue(modelLocale, null)?.ToString();

                //ignore the property which already has a value
                if (!string.IsNullOrEmpty(translated))
                    continue;

                //get original text to translate
                var originText = modelProperties.FirstOrDefault(p => p.Name.Equals(prop.Name))?.GetValue(model, null)?.ToString();

                //ignore the empty original text
                if (string.IsNullOrEmpty(originText))
                    continue;

                try
                {
                    var translatedText = await TranslateAsync(originalLanguage, originText, translateToLanguage, properties[prop.Name]);
                    
                    //set translated text to property
                    prop.SetValue(modelLocale, translatedText);
                    result.Add($"{nameof(model.Locales)}_{position}__{prop.Name}", translatedText);
                }
                catch (Exception e)
                {
                    var serviceName = await _localizationService.GetLocalizedEnumAsync((TranslationServiceType)_translationSettings.TranslationServiceId, 0);
                    await _logger.ErrorAsync($"{serviceName}: {e.Message}", e);

                    //DeepL: stop translate if one of the languages aren't support
                    //to reduce error count
                    if (e.Message.Contains("Value for 'target_lang' not supported", StringComparison.InvariantCultureIgnoreCase) || e.Message.Contains("Value for 'source_lang' not supported", StringComparison.InvariantCultureIgnoreCase))
                        break;
                }
            }
        }

        return result;

        //filter for get only string property which should be translated
        bool propertyFilter(PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType == typeof(string) && properties.ContainsKey(propertyInfo.Name);
        }
    }

    #endregion
}