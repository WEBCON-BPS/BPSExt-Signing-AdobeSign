using System;
using System.IO;
using System.Text;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;
using WebCon.WorkFlow.SDK.Documents;
using WebCon.WorkFlow.SDK.Documents.Model.Attachments;
using WebCon.BpsExt.Signing.AdobeSign.CustomActions.Helpers;
using System.Threading.Tasks;
using WebCon.WorkFlow.SDK.Tools.Other;

namespace WebCon.BpsExt.Signing.AdobeSign.CustomActions.Download
{
    public class DownloadDocAction : CustomAction<DownloadDocActionConfig>
    {
        public override async Task RunAsync(RunCustomActionParams args)
        {
            var log = new StringBuilder();
            try
            {
                var status = args.Context.CurrentDocument.GetFieldValue(Configuration.InputParams.StatusFieldId)?.ToString();
                if (!string.IsNullOrEmpty(status) && status.Equals(Models.Statuses.Signed, StringComparison.InvariantCultureIgnoreCase))
                {
                    var docId = args.Context.CurrentDocument.GetFieldValue(Configuration.InputParams.OperationFieldId)?.ToString();
                    var api = new AdobeSignHelper(log, args.Context);
                    var content = await api.GetDocumentAsync(docId, Configuration.ApiConfig.TokenValue);
                    await SaveAttAsync(args.Context, content);
                }
                else
                {
                    args.HasErrors = true;
                    args.Message = "Document cannot be a saved if it is not signed";
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

        private async Task SaveAttAsync(ActionContextInfo context, byte[] newAttContent)
        {
            var sourceAttData = context.CurrentDocument.GetFieldValue(Configuration.AttConfig.AttTechnicalFieldID)?.ToString();
            var sourceAtt = await context.CurrentDocument.Attachments.GetByIDAsync(Convert.ToInt32(sourceAttData));
            sourceAtt.SetContent(newAttContent);
            if (sourceAtt.FileExtension.Equals(".pdf", StringComparison.InvariantCultureIgnoreCase))
                sourceAtt.FileName = $"{Path.GetFileNameWithoutExtension(sourceAtt.FileName)}{Configuration.AttConfig.AttSufix}{sourceAtt.FileExtension}";
            else
                sourceAtt.FileName = $"{Path.GetFileNameWithoutExtension(sourceAtt.FileName)}{Configuration.AttConfig.AttSufix}.pdf";
           
            if (!string.IsNullOrEmpty(Configuration.AttConfig.SaveCategory))
                await SetFileGroup(sourceAtt, Configuration.AttConfig.SaveCategory);

            var manager = new DocumentAttachmentsManager(context);
            await manager.UpdateAttachmentAsync(new UpdateAttachmentParams()
            {
                Attachment = sourceAtt
            });
        }

        private async Task SetFileGroup(NewAttachmentData newAtt, string category)
        {
            if (category.Contains("#"))
            {
                newAtt.FileGroup = new AttachmentsGroup(TextHelper.GetPairId(category), TextHelper.GetPairName(category));
                return;
            }

            var fileGroup = await newAtt.ResolveAsync(category);
            newAtt.FileGroup = fileGroup ?? new AttachmentsGroup(category);
        }
    }
}