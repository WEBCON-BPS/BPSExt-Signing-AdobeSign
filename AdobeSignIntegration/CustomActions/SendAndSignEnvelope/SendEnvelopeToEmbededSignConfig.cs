using WebCon.BpsExt.Signing.AdobeSign.CustomActions.Models.Configuration;
using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace WebCon.BpsExt.Signing.AdobeSign.CustomActions.SendAndSignEnvelope
{
    public class SendEnvelopeToEmbededSignConfig : PluginConfiguration
    {
        [ConfigGroupBox(DisplayName = "AdobeSign API Settings")]
        public ApiConfiguration ApiConfig { get; set; }

        [ConfigGroupBox(DisplayName = "Attachment selection")]
        public InputAttConfig AttConfig { get; set; }

        [ConfigGroupBox(DisplayName = "Recipients selection")]
        public ItemListConfig Users { get; set; }

        [ConfigGroupBox(DisplayName = "Message content")]
        public MessageContent MessageContent { get; set; }

        [ConfigEditableText(DisplayName = "Url to the form", IsRequired = true)]
        public string RedirectUrl { get; set; }
    }

    public class ApiConfiguration
    {
        [ConfigEditableText(DisplayName = "Integration Key", Description = "Create a integration key: https://adobe.lookbookhq.com/sign-recommend/ACMIG04", IsRequired = true)]
        public string TokenValue { get; set; }
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
        [ConfigEditableText(DisplayName = "Signer Name", IsRequired = true)]
        public string SignerName { get; set; }

        [ConfigEditableText(DisplayName = "Signer Mail", IsRequired = true)]
        public string SignerMail { get; set; }

        [ConfigEditableText(DisplayName = "Signer phone number", IsRequired = true)]
        public string SignerPhoneNumber { get; set; }

        [ConfigEditableText(DisplayName = "Country Code", Description = "The numeric country calling code (ISD code) required for the participant to view and sign the document if authentication type is PHONE")]
        public string CountryCode { get; set; }

        [ConfigEditableText(DisplayName = "Country ISO Code", Description = "The country ISO Alpha-2 code required for the participant to view and sign the document if authentication method is PHONE")]
        public string IsoCountryCode { get; set; }

        [ConfigEditableBool(DisplayName = "Additional SMS verification", Description = "Mark this field if additional verification should be required.")]
        public bool PhoneAutorization { get; set; }
    } 
}