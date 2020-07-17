using System;
using System.IO;
using System.Text;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;
using WebCon.WorkFlow.SDK.Documents;
using WebCon.WorkFlow.SDK.Documents.Model.Attachments;
using WebCon.BpsExt.Signing.AdobeSign.CustomActions.Helpers;

namespace WebCon.BpsExt.Signing.AdobeSign.CustomActions.Download
{
    public class DownloadDocAction : CustomAction<DownloadDocActionConfig>
    {
        public override void Run(RunCustomActionParams args)
        {
            var log = new StringBuilder();
            try
            {
                var status = args.Context.CurrentDocument.GetFieldValue(Configuration.InputParams.StatusFieldId)?.ToString();
                if (!string.IsNullOrEmpty(status) && status.Equals(Models.Statuses.Signed, StringComparison.InvariantCultureIgnoreCase))
                {
                    var docId = args.Context.CurrentDocument.GetFieldValue(Configuration.InputParams.OperationFieldId)?.ToString();
                    var api = new AdobeSignHelper(log);
                    var content = api.GetDocument(docId, Configuration.ApiConfig.TokenValue);
                    SaveAtt(args.Context.CurrentDocument, content);
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
                args.Context.PluginLogger.AppendInfo(log.ToString());
            }
        }

        private void SaveAtt(WorkFlow.SDK.Documents.Model.CurrentDocumentData currentDocument, byte[] newAttContent)
        {
            var sourceAttData = currentDocument.GetFieldValue(Configuration.AttConfig.AttTechnicalFieldID)?.ToString();
            var sourceAtt = currentDocument.Attachments.GetByID(Convert.ToInt32(sourceAttData));
            sourceAtt.Content = newAttContent;
            if (sourceAtt.FileExtension.Equals(".pdf", StringComparison.InvariantCultureIgnoreCase))
                sourceAtt.FileName = $"{Path.GetFileNameWithoutExtension(sourceAtt.FileName)}{Configuration.AttConfig.AttSufix}{sourceAtt.FileExtension}";
            else
                sourceAtt.FileName = $"{Path.GetFileNameWithoutExtension(sourceAtt.FileName)}{Configuration.AttConfig.AttSufix}.pdf";
           
            if (!string.IsNullOrEmpty(Configuration.AttConfig.SaveCategory))
            {              
                sourceAtt.FileGroup = new AttachmentsGroup(Configuration.AttConfig.SaveCategory, null);
            }            

            DocumentAttachmentsManager.UpdateAttachment(new UpdateAttachmentParams()
            {
                Attachment = sourceAtt
            });
        }
    }
}