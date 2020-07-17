using System;
using System.Collections.Generic;
using System.Text;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;
using WebCon.BpsExt.Signing.AdobeSign.CustomActions.Helpers;
using WebCon.WorkFlow.SDK.Documents.Model.Attachments;
using WebCon.WorkFlow.SDK.Documents.Model;

namespace WebCon.BpsExt.Signing.AdobeSign.CustomActions.SendAndSignEnvelope
{
    public class SendEnvelopeToEmbededSign : CustomAction<SendEnvelopeToEmbededSignConfig>
    {
        const string SignerRole = "SIGNER";
        const string AuthMethod = "PHONE";
        StringBuilder _log = new StringBuilder();

        public override void Run(RunCustomActionParams args)
        {           
            try
            {
                var att = AttachmentHelper.GetAttachment(args.Context, Configuration.AttConfig, _log);
                if (att == null)
                    throw new Exception("No attachment to signature");              

                var signUrl = CallAdobeApi(att, args.Context.CurrentDocument);                
                _log.AppendLine($"Document URL: {signUrl}");
                args.TransitionInfo.RedirectUrl(signUrl);
            }
            catch (Exception e)
            {
                _log.AppendLine(e.ToString());
                args.HasErrors = true;
                args.Message = e.Message;
            }
            finally
            {
                args.LogMessage = _log.ToString();
                args.Context.PluginLogger.AppendInfo(_log.ToString());
            }
        }

        private string CallAdobeApi(AttachmentData att, DocumentData currentDocument)
        {
            var api = new AdobeSignHelper(_log);
            var documentId = api.SendDocument(att.Content, Configuration.ApiConfig.TokenValue, $"{att.FileName}.{att.FileExtension}");
            var agreementsId = api.SendToSig(documentId, Configuration.ApiConfig.TokenValue, Configuration.MessageContent.MailSubject, Configuration.MessageContent.MailBody, GetMemberInfo(), true, Configuration.RedirectUrl);

            currentDocument.SetFieldValue(Configuration.AttConfig.AgreementsIdFild, agreementsId);
            currentDocument.SetFieldValue(Configuration.AttConfig.AttTechnicalFieldID, att.ID);

            return api.GetSigningURL(Configuration.ApiConfig.TokenValue, agreementsId);
        }

        private List<Models.Send.Participantsetsinfo> GetMemberInfo()
        {
            int order = 1;
            var result = new List<Models.Send.Participantsetsinfo>();

            var member = new Models.Send.Participantsetsinfo();
            var singleUser = new Models.Send.Memberinfo();
            member.name = Configuration.Users.SignerName;
            member.role = SignerRole;
            member.order = order++;
            singleUser.email = Configuration.Users.SignerMail;
            if (Configuration.Users.PhoneAutorization)
            {
                singleUser.securityOption = new Models.Send.Securityoption1();
                singleUser.securityOption.authenticationMethod = AuthMethod;
                singleUser.securityOption.phoneInfo = new Models.Send.Phoneinfo()
                {
                    phone = Configuration.Users.SignerPhoneNumber,
                    countryCode = Configuration.Users.CountryCode ?? "",
                    countryIsoCode = Configuration.Users.IsoCountryCode ?? ""
                };
            }
            
            member.memberInfos = new Models.Send.Memberinfo[] { singleUser };
            result.Add(member);

            return result;
        }
    }
}