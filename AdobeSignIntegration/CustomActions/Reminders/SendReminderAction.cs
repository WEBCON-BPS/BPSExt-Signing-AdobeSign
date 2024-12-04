using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;
using WebCon.WorkFlow.SDK.Documents.Model.ItemLists;
using WebCon.BpsExt.Signing.AdobeSign.CustomActions.Helpers;
using System.Threading.Tasks;

namespace WebCon.BpsExt.Signing.AdobeSign.CustomActions.Reminders
{
    public class SendReminderAction : CustomAction<SendReminderActionConfig>
    {
        public override async Task RunAsync(RunCustomActionParams args)
        {
            var log = new StringBuilder();
            try
            {
                var status = args.Context.CurrentDocument.GetFieldValue(Configuration.ApiConfig.StatusFildId)?.ToString();
                if (!string.IsNullOrEmpty(status) && !status.Equals(Models.Statuses.Signed, StringComparison.InvariantCultureIgnoreCase))
                {
                    var docId = args.Context.CurrentDocument.GetFieldValue(Configuration.ApiConfig.OperationFildId)?.ToString();
                    var api = new AdobeSignHelper(log, args.Context, Configuration.ApiConfig);
                    var membersInfo = await api.GetMembersIdsAsync(docId);
                    var users = GetUsers(args.Context.CurrentDocument.ItemsLists.GetByID(Configuration.Users.SignersList.ItemListId), membersInfo.ToList());
                    await api.SendRemindAsync(docId, users);
                }
                else
                {
                    args.HasErrors = true;
                    args.Message = "The document has already been signed";
                }
            }
            catch (Exception e)
            {
                args.HasErrors = true;
                args.Message = e.Message;
                log.AppendLine(e.ToString());
            }
            finally
            {
                args.LogMessage = log.ToString();
                args.Context.PluginLogger?.AppendInfo(log.ToString());
            }
        }       

        private List<string> GetUsers(ItemsList itemsList, List<Models.Remind.Memberinfo> info)
        {
            var ids = new List<string>();
            foreach (var row in itemsList.Rows)
            {
                if (info.Any(x => x.email == row.GetCellValue(Configuration.Users.SignersList.SignerMailColumnID).ToString()))
                    ids.Add(info.FirstOrDefault(x => x.email == row.GetCellValue(Configuration.Users.SignersList.SignerMailColumnID).ToString()).id);
            }

            if(ids.Count <= 0)
                throw new Exception("Empty signers list");

            return ids;
        }
    }
}