using WebCon.BpsExt.Signing.AdobeSign.CustomActions.Models.Configuration;
using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace WebCon.BpsExt.Signing.AdobeSign.CustomActions.SendEnvelope
{
    public class SendEnvelopeActionConfig : PluginConfiguration
    {    
        [ConfigGroupBox(DisplayName = "AdobeSign API Settings")]
        public BaseConfiguration ApiConfig { get; set; }

        [ConfigGroupBox(DisplayName = "Attachment selection")]
        public InputAttConfig AttConfig { get; set; }

        [ConfigGroupBox(DisplayName = "Recipients selection")]
        public ItemListConfig Users { get; set; }

        [ConfigGroupBox(DisplayName = "Message content")]
        public MessageContent MessageContent { get; set; }       
    }

    public class MessageContent
    {
        [ConfigEditableText(DisplayName = "Subject", IsRequired = true)]
        public string MailSubject { get; set; }

        [ConfigEditableText(DisplayName = "Content", IsRequired = true, Multiline = true)]
        public string MailBody { get; set; }
    }

  

    public class ItemListConfig
    {
        [ConfigEditableItemList(DisplayName = "Signers Item List")]
        public SignersList SignersList { get; set; }

        [ConfigEditableText(DisplayName = "Country Code", Description = "The numeric country calling code (ISD code) required for the participant to view and sign the document if authentication type is PHONE")]
        public string CountryCode { get; set; }

        [ConfigEditableText(DisplayName = "Country ISO Code", Description = "The country ISO Alpha-2 code required for the participant to view and sign the document if authentication method is PHONE")]
        public string IsoCountryCode { get; set; }

        [ConfigEditableBool(DisplayName = "Additional SMS verification", Description = "Mark this field if additional verification should be required.")]
        public bool PhoneAutorization { get; set; }
    }

    public class SignersList : IConfigEditableItemList
    {
        public int ItemListId { get; set; }

        [ConfigEditableItemListColumnID(DisplayName = "Name", IsRequired = true)]
        public int SignerNameColumnID { get; set; }

        [ConfigEditableItemListColumnID(DisplayName = "E-mail", IsRequired = true)]
        public int SignerMailColumnID { get; set; }

        [ConfigEditableItemListColumnID(DisplayName = "Phone Number", IsRequired = true)]
        public int SignerPhoneNumberColumnID { get; set; }

    }
}