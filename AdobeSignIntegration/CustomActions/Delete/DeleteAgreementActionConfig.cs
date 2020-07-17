using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace WebCon.BpsExt.Signing.AdobeSign.CustomActions.Delete
{
    public class DeleteAgreementActionConfig : PluginConfiguration
    {
        [ConfigEditableText(DisplayName = "Integration Key", Description = "Create a integration key: https://adobe.lookbookhq.com/sign-recommend/ACMIG04", IsRequired = true)]
        public string TokenValue { get; set; }

        [ConfigEditableFormFieldID(DisplayName = "Document ID", Description = "Select the text field where the Document ID was saved")]
        public int OperationFildId { get; set; }
    }
}