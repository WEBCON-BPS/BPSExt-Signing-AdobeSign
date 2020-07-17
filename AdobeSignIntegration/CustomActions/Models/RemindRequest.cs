namespace WebCon.BpsExt.Signing.AdobeSign.CustomActions.Models.Remind
{
    public class RemindRequest
    {
        public string[] recipientParticipantIds { get; set; }
        public string status { get; set; }
    }


    public class ResponseMembersModel
    {
        public Participantset[] participantSets { get; set; }
        public Senderinfo senderInfo { get; set; }
    }

    public class Senderinfo
    {
        public string participantId { get; set; }
        public string email { get; set; }
        public string company { get; set; }
        public string name { get; set; }
        public bool self { get; set; }
        public bool hidden { get; set; }
        public string status { get; set; }
    }

    public class Participantset
    {
        public Memberinfo[] memberInfos { get; set; }
        public string id { get; set; }
        public string role { get; set; }
        public string status { get; set; }
        public int order { get; set; }
    }

    public class Memberinfo
    {
        public string id { get; set; }
        public string email { get; set; }
        public bool self { get; set; }
        public Securityoption securityOption { get; set; }
        public string status { get; set; }
    }

    public class Securityoption
    {
        public string authenticationMethod { get; set; }
    }
}
