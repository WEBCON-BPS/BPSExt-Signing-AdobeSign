using WebCon.BpsExt.Signing.AdobeSign.CustomActions.Models.Configuration;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace WebCon.BpsExt.Signing.AdobeSign.CustomActions.AllStatus
{
    public class CheckAllDocStatusActionConfig : BaseConfiguration
    {

        [ConfigGroupBox(DisplayName = "Workflow section")]
        public WorkflowConfig Workflow { get; set; }      
    }

    public class WorkflowConfig
    {
        [ConfigEditableText(DisplayName = "Step ID", IsRequired = true)]
        public int StepId { get; set; }

        [ConfigEditableText(DisplayName = "Success Path ID", IsRequired = true)]
        public int SuccessPathId { get; set; }

        [ConfigEditableText(DisplayName = "Incorrect Path ID", IsRequired = true)]
        public int ErrorPathId { get; set; }

        [ConfigEditableText(DisplayName = "Document ID field name", Description = "Select the database field name where the Document ID was saved.", IsRequired = true)]
        public string OperationFieldName { get; set; }

        [ConfigEditableText(DisplayName = "Execution Time", Description = "The maximum execution time, in seconds", DefaultText = "120")]
        public int ExecutionTime { get; set; }
    }
}