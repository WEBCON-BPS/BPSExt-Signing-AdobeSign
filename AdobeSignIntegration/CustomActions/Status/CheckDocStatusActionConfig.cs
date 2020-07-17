using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace WebCon.BpsExt.Signing.AdobeSign.CustomActions.Status
{
    public class CheckDocStatusActionConfig : PluginConfiguration
    {
        [ConfigEditableText(DisplayName = "Integration Key", Description = "Create a integration key: https://adobe.lookbookhq.com/sign-recommend/ACMIG04", IsRequired = true)]
        public string TokenValue { get; set; }

        [ConfigEditableFormFieldID(DisplayName = "Document ID", Description = "Select the text field where the Document ID was saved")]
        public int DokFildId { get; set; }

        [ConfigEditableFormFieldID(DisplayName = "Copy Status to field", Description = "Specify a text field on the form where external documents status will be saved")]
        public int StatusFildId { get; set; }
      
    }
}