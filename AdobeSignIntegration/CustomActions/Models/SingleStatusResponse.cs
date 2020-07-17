using System;

namespace WebCon.BpsExt.Signing.AdobeSign.CustomActions.Models
{
    public class SingleStatusResponse
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public Participantsetsinfo[] participantSetsInfo { get; set; }
        public string senderEmail { get; set; }
        public DateTime createdDate { get; set; }
        public string signatureType { get; set; }
        public string locale { get; set; }
        public string status { get; set; }
        public bool documentVisibilityEnabled { get; set; }
        public bool hasFormFieldData { get; set; }
        public bool hasSignerIdentityReport { get; set; }
        public bool documentRetentionApplied { get; set; }
    }

    public class Participantsetsinfo
    {
        public Memberinfo[] memberInfos { get; set; }
        public string role { get; set; }
        public int order { get; set; }
    }

    public class Memberinfo
    {
        public string email { get; set; }
        public Securityoption securityOption { get; set; }
    }

    public class Securityoption
    {
        public string authenticationMethod { get; set; }
    }
}
