namespace WebCon.BpsExt.Signing.AdobeSign.CustomActions.Models.Self
{
    public class SelfSignResponse
    {
        public Signingurlsetinfo[] signingUrlSetInfos { get; set; }
    }

    public class Signingurlsetinfo
    {
        public Signingurl[] signingUrls { get; set; }
    }

    public class Signingurl
    {
        public string email { get; set; }
        public string esignUrl { get; set; }
    }
}
