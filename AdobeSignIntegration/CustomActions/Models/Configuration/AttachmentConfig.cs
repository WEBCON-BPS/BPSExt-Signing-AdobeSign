using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace WebCon.BpsExt.Signing.AdobeSign.CustomActions.Models.Configuration
{
    public class InputAttConfig
    {
        [ConfigEditableEnum(DisplayName = "Selection mode", Description = "The attachments to sign can be selected by category ID and regex (optional) or by SQL query", DefaultValue = 0)]
        public InputType InputAttType { get; set; }

        [ConfigEditableEnum(DisplayName = "Category mode", Description = "Select ‘None’ for files not associated with any category or ‘All’ for attachment from the element.", DefaultValue = 0)]
        public CategoryType CatType { get; set; }

        [ConfigEditableText(DisplayName = "Category ID", Description = "Select the attachment category ID to be signed.")]
        public int InputCategoryId { get; set; }

        [ConfigEditableText(DisplayName = "Regex expression", Description = "Regular expression can be used as an additional filter for attachments from the selected category.", DefaultText = ".*[.]pdf")]
        public string AttRegularExpression { get; set; }

        [ConfigEditableText(DisplayName = "SQL query", Description = @"Query should return a list of attachments' IDs from WFDataAttachmets table.
Example: Select [ATT_ID] from [WFDataAttachmets] Where [ATT_Name] = 'agreement.pdf'", Multiline = true, TagEvaluationMode = EvaluationMode.SQL, Lines = 4)]
        public string AttQuery { get; set; }

        [ConfigEditableFormFieldID(DisplayName = "Copy source attachment ID", Description = "Specify a text field on the form where source attachment ID will be saved")]
        public int AttTechnicalFieldID { get; set; }

        [ConfigEditableFormFieldID(DisplayName = "Copy sent document ID to field", Description = "Specify a text field on the form where external documents ID will be saved", IsRequired = true)]
        public int AgreementsIdFild { get; set; }
    }

    public enum InputType
    {
        Category,
        SQL
    }

    public enum CategoryType
    {
        ID,
        All,
        None
    }
}
