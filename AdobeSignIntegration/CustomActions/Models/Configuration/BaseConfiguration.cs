using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace WebCon.BpsExt.Signing.AdobeSign.CustomActions.Models.Configuration
{
    public class BaseConfiguration : PluginConfiguration
    {
        [ConfigEditableText(DisplayName = "Integration Key", Description = "Create a integration key: https://adobe.lookbookhq.com/sign-recommend/ACMIG04", IsRequired = true)]
        public string TokenValue { get; set; }

        [ConfigEditableBool("Use proxy")]
        public bool UseProxy { get; set; }
    }
}
