using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WebCon.BpsExt.Signing.AdobeSign.CustomActions.Models.Configuration;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;
using WebCon.WorkFlow.SDK.Documents;
using WebCon.WorkFlow.SDK.Documents.Model.Attachments;
using WebCon.WorkFlow.SDK.Tools.Data;

namespace WebCon.BpsExt.Signing.AdobeSign.CustomActions.Helpers
{
    internal sealed class AttachmentHelper
    {
        internal static AttachmentData GetAttachment(ActionContextInfo context, InputAttConfig config, StringBuilder log)
        {
            var manager = new DocumentAttachmentsManager(context);

            if (config.InputAttType == InputType.Category)
            {
                log.AppendLine("Downloading attachments by category");
           
                var allAttachments = manager.GetAttachments(new GetAttachmentsParams()
                {
                    DocumentId = context.CurrentDocument.ID,
                    IncludeContent = true
                });

                if (config.CatType == CategoryType.ID)
                {
                    return allAttachments.FirstOrDefault(x =>
                    x.FileGroup != null &&
                    x.FileGroup.ID == config.InputCategoryId.ToString() &&
                    (string.IsNullOrEmpty(config.AttRegularExpression) || Regex.IsMatch(x.FileName, config.AttRegularExpression)));
                }
                else if (config.CatType == CategoryType.None)
                {
                    return allAttachments.FirstOrDefault(x =>
                    x.FileGroup == null
                    && (string.IsNullOrEmpty(config.AttRegularExpression) || Regex.IsMatch(x.FileName, config.AttRegularExpression)));
                }
                else
                {
                    return allAttachments.First();
                }
            }
            else
            {
                log.AppendLine("Downloading attachments by SQL query");

                var attId = SqlExecutionHelper.ExecSqlCommandScalar(config.AttQuery, context);
                if (attId == null)
                    throw new Exception("Sql query not returning result");

                return manager.GetAttachment(Convert.ToInt32(attId));
            }
        }
    }
}
