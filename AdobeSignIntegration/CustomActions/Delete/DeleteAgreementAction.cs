using System;
using System.Text;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;
using WebCon.BpsExt.Signing.AdobeSign.CustomActions.Helpers;
using System.Threading.Tasks;

namespace WebCon.BpsExt.Signing.AdobeSign.CustomActions.Delete
{
    public class DeleteAgreementAction : CustomAction<DeleteAgreementActionConfig>
    {
        public override async Task RunAsync(RunCustomActionParams args)
        {
            var log = new StringBuilder();
            try
            {
                var operationId = args.Context.CurrentDocument.GetFieldValue(Configuration.OperationFildId)?.ToString();
                var api = new AdobeSignHelper(log, args.Context);
                await api.DeleteAgreementAsync(operationId, Configuration.TokenValue);
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
    }
}