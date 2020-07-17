using System;

namespace WebCon.BpsExt.Signing.AdobeSign.CustomActions.Models
{
    public class Agreements
    {
        public Useragreementlist[] userAgreementList { get; set; }
        public Page page { get; set; }
    }

    public class Page
    {
    }

    public class Useragreementlist
    {
        public string id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public DateTime displayDate { get; set; }
        public Displayparticipantsetinfo[] displayParticipantSetInfos { get; set; }
        public string latestVersionId { get; set; }
        public string status { get; set; }
        public bool esign { get; set; }
        public bool hidden { get; set; }
    }

    public class Displayparticipantsetinfo
    {
        public Displayusersetmemberinfo[] displayUserSetMemberInfos { get; set; }
    }

    public class Displayusersetmemberinfo
    {
        public string email { get; set; }
        public string fullName { get; set; }
        public string company { get; set; }
    }

}
