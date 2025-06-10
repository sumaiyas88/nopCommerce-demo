using Nop.Web.Framework.Models;

namespace Nop.Web.Framework.Translation;

/// <summary>
/// Translation interface
/// </summary>
public partial interface ITranslationService
{
    /// <summary>
    /// Translate the properties
    /// </summary>
    /// <param name="model">The localized model to translate</param>
    /// <param name="properties">List of properties which should be translated</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task<Dictionary<string, string>> TranslateAsync<T>(ILocalizedModel<T> model, PropertyToTranslate[] properties) where T : ILocalizedLocaleModel;

    /// <summary>
    /// Represent information about property which should be translated
    /// </summary>
    public partial class PropertyToTranslate
    {
        public PropertyToTranslate(string propertyName, bool isHtml = false)
        {
            PropertyName = propertyName;
            IsHtml = isHtml;
        }

        /// <summary>
        /// Gets or sets the property name
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the value which indicate is this property should consider as HTML
        /// </summary>
        public bool IsHtml { get; set; }
    }
}