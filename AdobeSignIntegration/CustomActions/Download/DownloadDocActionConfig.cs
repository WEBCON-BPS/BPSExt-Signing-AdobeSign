using WebCon.BpsExt.Signing.AdobeSign.CustomActions.Models.Configuration;
using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace WebCon.BpsExt.Signing.AdobeSign.CustomActions.Download
{
    public class DownloadDocActionConfig : PluginConfiguration
    {
        [ConfigGroupBox(DisplayName = "AdobeSign API Settings")]
        public BaseConfiguration ApiConfig { get; set; }

        [ConfigGroupBox(DisplayName = "Attachment selection")]
        public AttConfig AttConfig { get; set; }

        [ConfigGroupBox(DisplayName = " Input Settings")]
        public InputParameters InputParams { get; set; }
    }

    public class AttConfig
    {
        [ConfigEditableText(DisplayName = "Suffix", Description = "Suffix that will be added to the name of the downloaded file. When this field is empty then the attachment will be overwritten (if the attachment with the selected Document ID exists on the form).", DefaultText = "_sign")]
        public string AttSufix { get; set; }

        [ConfigEditableText(DisplayName = "Category", Description = "Attachment category where the signed documents will be downloaded")]
        public string SaveCategory { get; set; }

        [ConfigEditableFormFieldID(DisplayName = "Copy source attachment ID", Description = "Select the technical field where the source attachment ID was saved.")]
        public int AttTechnicalFieldID { get; set; }
    }

    public class InputParameters
    {
        [ConfigEditableFormFieldID(DisplayName = "Document ID", Description = "Select the text field where the Document ID was saved.")]
        public int OperationFieldId { get; set; }

        [ConfigEditableFormFieldID(DisplayName = "Status", Description = "Select the text field where the Document status was saved.")]
        public int StatusFieldId { get; set; }
    }
}