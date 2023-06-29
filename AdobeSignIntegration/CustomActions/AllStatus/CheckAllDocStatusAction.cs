using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCon.BpsExt.Signing.AdobeSign.CustomActions.Helpers;
using WebCon.BpsExt.Signing.AdobeSign.CustomActions.Models;
using WebCon.WorkFlow.SDK.ActionPlugins;
using WebCon.WorkFlow.SDK.ActionPlugins.Model;
using WebCon.WorkFlow.SDK.Documents;
using WebCon.WorkFlow.SDK.Documents.Model;
using WebCon.WorkFlow.SDK.Tools.Data;

namespace WebCon.BpsExt.Signing.AdobeSign.CustomActions.AllStatus
{
    public class CheckAllDocStatusAction : CustomAction<CheckAllDocStatusActionConfig>
    {
        public override ActionTriggers AvailableActionTriggers => ActionTriggers.Recurrent;
               
        public override async Task RunWithoutDocumentContextAsync(RunCustomActionWithoutContextParams args)
        {
            var log = new StringBuilder();
            try
            {
                var api = new AdobeSignHelper(log, args.Context); 
                var agreements = await api.GetAllStatusAsync(Configuration.TokenValue);
                await CheckAndMoveElementsAsync(agreements.userAgreementList?.ToList(), args.Context);
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

        private async Task CheckAndMoveElementsAsync(List<Useragreementlist> agreements, ActionWithoutDocumentContext context)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var sqlQuery = $"SELECT WFD_ID, {Configuration.Workflow.OperationFieldName} " +
                           $"FROM WFElements WHERE WFD_STPID = {Configuration.Workflow.StepId}";

            var dt = await new SqlExecutionHelper(context).GetDataTableForSqlCommandOutsideTransactionAsync(sqlQuery);
            await CheckDocumentsStatusAsync(dt, agreements, sw, context);
        }

        private async Task CheckDocumentsStatusAsync(DataTable dt, List<Useragreementlist> agreements, Stopwatch sw, ActionWithoutDocumentContext context)
        {
            var time = TimeSpan.FromSeconds(Configuration.Workflow.ExecutionTime);

            foreach (DataRow row in dt.Rows)
            {
                if (sw.ElapsedMilliseconds > time.TotalMilliseconds)
                    break;

                var wfdId = Convert.ToInt32(row["WFD_ID"]);
                var docId = row[Configuration.Workflow.OperationFieldName].ToString();

                if (agreements.Any(x => x.id == docId))
                {
                    var signStatus = agreements.FirstOrDefault(x => x.id == docId).status;
                    switch (signStatus)
                    {
                        case Statuses.Signed:
                            await MoveDocumentAsync(wfdId, Configuration.Workflow.SuccessPathId, context);
                            break;
                        case Statuses.Cancelled:
                        case Statuses.Expired:
                            await MoveDocumentAsync(wfdId, Configuration.Workflow.ErrorPathId, context);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private async Task MoveDocumentAsync(int docId, int path, ActionWithoutDocumentContext context)
        {
            var manager = new DocumentsManager(context);
            var document = await manager.GetDocumentByIdAsync(docId, true);
            await manager.MoveDocumentToNextStepAsync(new MoveDocumentToNextStepParams(document, path)
            {
                ForceCheckout = true,
                SkipPermissionsCheck = true
            });
        }
    }
}