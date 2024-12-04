using WebCon.BpsExt.Signing.AdobeSign.CustomActions.Models.Configuration;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace WebCon.BpsExt.Signing.AdobeSign.CustomActions.Delete
{
    public class DeleteAgreementActionConfig : BaseConfiguration
    {
        [ConfigEditableFormFieldID(DisplayName = "Document ID", Description = "Select the text field where the Document ID was saved")]
        public int OperationFildId { get; set; }
    }
}