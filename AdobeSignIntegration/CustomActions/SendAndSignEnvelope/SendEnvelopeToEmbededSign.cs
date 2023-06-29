using System;
using System.Collections.Generic;
using System.Text;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;
using WebCon.BpsExt.Signing.AdobeSign.CustomActions.Helpers;
using WebCon.WorkFlow.SDK.Documents.Model.Attachments;
using WebCon.WorkFlow.SDK.Documents.Model;
using System.Threading.Tasks;

namespace WebCon.BpsExt.Signing.AdobeSign.CustomActions.SendAndSignEnvelope
{
    public class SendEnvelopeToEmbededSign : CustomAction<SendEnvelopeToEmbededSignConfig>
    {
        const string SignerRole = "SIGNER";
        const string AuthMethod = "PHONE";
        StringBuilder _log = new StringBuilder();

        public override async Task RunAsync(RunCustomActionParams args)
        {           
            try
            {
                var att = await AttachmentHelper.GetAttachmentAsync(args.Context, Configuration.AttConfig, _log);
                if (att == null)
                    throw new Exception("No attachment to signature");              

                var signUrl = await CallAdobeApiAsync(att, args.Context);                
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
                args.Context.PluginLogger?.AppendInfo(_log.ToString());
            }
        }

        private async Task<string> CallAdobeApiAsync(AttachmentData att, ActionContextInfo context)
        {
            var api = new AdobeSignHelper(_log, context);
            var documentId = await api.SendDocumentAsync(att.Content, Configuration.ApiConfig.TokenValue, $"{att.FileName}.{att.FileExtension}");
            var agreementsId = await api.SendToSigAsync(documentId, Configuration.ApiConfig.TokenValue, Configuration.MessageContent.MailSubject, Configuration.MessageContent.MailBody, GetMemberInfo(), true, Configuration.RedirectUrl);

            context.CurrentDocument.SetFieldValue(Configuration.AttConfig.AgreementsIdFild, agreementsId);
            context.CurrentDocument.SetFieldValue(Configuration.AttConfig.AttTechnicalFieldID, att.ID);

            return await api.GetSigningURLAsync(Configuration.ApiConfig.TokenValue, agreementsId);
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