using System;
using System.Text;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;
using WebCon.BpsExt.Signing.AdobeSign.CustomActions.Helpers;
using System.Threading.Tasks;

namespace WebCon.BpsExt.Signing.AdobeSign.CustomActions.Status
{
    public class CheckDocStatusAction : CustomAction<CheckDocStatusActionConfig>
    {
        public override async Task RunAsync(RunCustomActionParams args)
        {
            var log = new StringBuilder();
            try
            {
                var docId = args.Context.CurrentDocument.GetFieldValue(Configuration.DokFildId)?.ToString();
                var api = new AdobeSignHelper(log, args.Context);
                var status = await api.GetStatusAsync(docId, Configuration.TokenValue);
                args.Context.CurrentDocument.SetFieldValue(Configuration.StatusFildId, status);
            }
            catch(Exception e)
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