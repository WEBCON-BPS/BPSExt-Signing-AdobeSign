using System;
using System.Text;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;
using System.Collections.Generic;
using WebCon.WorkFlow.SDK.Documents.Model.ItemLists;
using WebCon.BpsExt.Signing.AdobeSign.CustomActions.Helpers;
using System.Threading.Tasks;

namespace WebCon.BpsExt.Signing.AdobeSign.CustomActions.SendEnvelope
{
    public class SendEnvelopeAction : CustomAction<SendEnvelopeActionConfig>
    {
        const string SignerRole = "SIGNER";
        const string AuthMethod = "PHONE";

        public override async Task RunAsync(RunCustomActionParams args)
        {
            var log = new StringBuilder();
            try
            {              
                var att = await AttachmentHelper.GetAttachmentAsync(args.Context, Configuration.AttConfig, log);
                if (att == null)
                    throw new Exception("No attachment to signature");

                var api = new AdobeSignHelper(log, args.Context);
                var docId = await api.SendDocumentAsync(att.Content, Configuration.ApiConfig.TokenValue, $"{att.FileName}.{att.FileExtension}");                           
                var operationId = await api.SendToSigAsync(docId, Configuration.ApiConfig.TokenValue, Configuration.MessageContent.MailSubject, Configuration.MessageContent.MailBody, GetMembersInfo(args.Context.CurrentDocument.ItemsLists.GetByID(Configuration.Users.SignersList.ItemListId)));

                args.Context.CurrentDocument.SetFieldValue(Configuration.AttConfig.AgreementsIdFild, operationId);
                args.Context.CurrentDocument.SetFieldValue(Configuration.AttConfig.AttTechnicalFieldID, att.ID);                
            }
            catch (Exception e)
            {
                log.AppendLine(e.ToString());
                args.HasErrors = true;
                args.Message = e.Message;
            }
            finally
            {
                args.LogMessage = log.ToString();
                args.Context.PluginLogger?.AppendInfo(log.ToString());
            }
        }
      
        private List<Models.Send.Participantsetsinfo> GetMembersInfo(ItemsList itemsList)
        {
            if (itemsList.Rows.Count <= 0)
                throw new Exception("Empty signers list");

            int order = 1;
            var result = new List<Models.Send.Participantsetsinfo>();
            foreach (var row in itemsList.Rows)
            {
                var member = new Models.Send.Participantsetsinfo();               
                var singleUser = new Models.Send.Memberinfo();
                member.name = row.GetCellValue(Configuration.Users.SignersList.SignerNameColumnID).ToString();
                member.role = SignerRole;
                member.order = order++;
                singleUser.email = row.GetCellValue(Configuration.Users.SignersList.SignerMailColumnID).ToString();        
                if (Configuration.Users.PhoneAutorization)
                {
                    singleUser.securityOption = new Models.Send.Securityoption1();
                    singleUser.securityOption.authenticationMethod = AuthMethod;
                    singleUser.securityOption.phoneInfo = new Models.Send.Phoneinfo()
                    {
                        phone = row.GetCellValue(Configuration.Users.SignersList.SignerPhoneNumberColumnID).ToString(),
                        countryCode = Configuration.Users.CountryCode ?? "",
                        countryIsoCode = Configuration.Users.IsoCountryCode ?? ""
                    };
                }

                member.memberInfos = new Models.Send.Memberinfo[] { singleUser };
                result.Add(member);
            }

            return result;
        }      
    }
}