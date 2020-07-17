namespace WebCon.BpsExt.Signing.AdobeSign.CustomActions.Models.Send
{
    public class SendRequest
    {
        public Fileinfo[] fileInfos { get; set; }
        public string name { get; set; }
        public Participantsetsinfo[] participantSetsInfo { get; set; }
        public string signatureType { get; set; }
        public string state { get; set; }
        public Cc[] ccs { get; set; }
        public string createdDate { get; set; }
        public Deviceinfo deviceInfo { get; set; }

        public Emailoption emailOption { get; set; }
        public string expirationTime { get; set; }
        public Externalid externalId { get; set; }

        public string groupId { get; set; }

        public string id { get; set; }

        public string locale { get; set; }
        public Mergefieldinfo[] mergeFieldInfo { get; set; }
        public string message { get; set; }
        public string parentId { get; set; }
        public Postsignoption postSignOption { get; set; }
        public string reminderFrequency { get; set; }
        public Securityoption securityOption { get; set; }
        public string senderEmail { get; set; }
        public string status { get; set; }
        public string type { get; set; }
        public Vaultinginfo vaultingInfo { get; set; }
        public string workflowId { get; set; }
    }

    public class Deviceinfo
    {
        public string applicationDescription { get; set; }
        public string deviceDescription { get; set; }
        public string deviceTime { get; set; }
    }

    public class Emailoption
    {
        public Sendoptions sendOptions { get; set; }
    }

    public class Sendoptions
    {
        public string completionEmails { get; set; }
        public string inFlightEmails { get; set; }
        public string initEmails { get; set; }
    }

    public class Externalid
    {
        public string id { get; set; }
    }

    public class Postsignoption
    {
        public int redirectDelay { get; set; }
        public string redirectUrl { get; set; }
    }

    public class Securityoption
    {
        public string openPassword { get; set; }
    }

    public class Vaultinginfo
    {
        public bool enabled { get; set; }
    }

    public class Fileinfo
    {
        public Document document { get; set; }
        public string label { get; set; }
        public string libraryDocumentId { get; set; }
        public string transientDocumentId { get; set; }
        public Urlfileinfo urlFileInfo { get; set; }
    }

    public class Document
    {
        public string id { get; set; }
        public string label { get; set; }
        public int numPages { get; set; }
        public string mimeType { get; set; }
        public string name { get; set; }
    }

    public class Urlfileinfo
    {
        public string mimeType { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Participantsetsinfo
    {
        public Memberinfo[] memberInfos { get; set; }
        public int order { get; set; }
        public string role { get; set; }
        public string label { get; set; }
        public string name { get; set; }
        public string privateMessage { get; set; }
        public string[] visiblePages { get; set; }
    }

    public class Memberinfo
    {
        public string email { get; set; }
        public Securityoption1 securityOption { get; set; }
    }

    public class Securityoption1
    {
        public string authenticationMethod { get; set; }
        public string password { get; set; }
        public Phoneinfo phoneInfo { get; set; }
    }

    public class Phoneinfo
    {
        public string countryCode { get; set; }
        public string countryIsoCode { get; set; }
        public string phone { get; set; }
    }

    public class Cc
    {
        public string email { get; set; }
        public string label { get; set; }
        public string[] visiblePages { get; set; }
    }

    public class Formfieldlayertemplate
    {
        public Document1 document { get; set; }
        public string label { get; set; }
        public string libraryDocumentId { get; set; }
        public string transientDocumentId { get; set; }
        public Urlfileinfo1 urlFileInfo { get; set; }
    }

    public class Document1
    {
        public string id { get; set; }
        public string label { get; set; }
        public int numPages { get; set; }
        public string mimeType { get; set; }
        public string name { get; set; }
    }

    public class Urlfileinfo1
    {
        public string mimeType { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Mergefieldinfo
    {
        public string defaultValue { get; set; }
        public string fieldName { get; set; }
    }

}
