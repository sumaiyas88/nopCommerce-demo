using Nop.Web.Framework.Models;

namespace Nop.Web.Framework.Factories;

/// <summary>
/// Translation interface
/// </summary>
public partial interface ITranslationFactory
{
    /// <summary>
    /// Translate the properties
    /// </summary>
    /// <param name="model">The localized model to translate</param>
    /// <param name="propertiesToTranslate">List of properties which should be translated</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task<Dictionary<string, string>> TranslateAsync<T>(ILocalizedModel<T> model, params (string PropertyName, bool IsHtml)[] propertiesToTranslate) where T : ILocalizedLocaleModel;
}