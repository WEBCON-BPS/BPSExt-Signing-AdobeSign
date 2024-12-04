using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System;
using WebCon.BpsExt.Signing.AdobeSign.CustomActions.Models;
using System.Threading;
using System.Threading.Tasks;
using WebCon.WorkFlow.SDK.Common.Model;
using WebCon.WorkFlow.SDK.Tools.Data;
using WebCon.BpsExt.Signing.AdobeSign.CustomActions.Models.Configuration;

namespace WebCon.BpsExt.Signing.AdobeSign.CustomActions.Helpers
{
    public class AdobeSignHelper
    {
        const string BaseUri = "https://secure.na1.echosign.com/api/rest/v6/baseUris";              
        StringBuilder _log;
        string _apiUrl;
        private readonly BaseContext _context;
        private string _token;
        private ConnectionsHelper _connectionsHelper;
        private bool _useProxy;

        public AdobeSignHelper(StringBuilder log, BaseContext context, BaseConfiguration config)
        {
            ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol |
                                                   SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;           
            _log = log;
            _context = context;
            _token = config.TokenValue;
            _useProxy = config.UseProxy;
            _connectionsHelper = new ConnectionsHelper(context);
        }

        private async Task<string> GetBaseUriAsync()
        {
            var client = GetClient(BaseUri);
            var request = new HttpRequestMessage(HttpMethod.Get, BaseUri);
            var response = await client.SendAsync(request);
            var jsonResult = await response.Content.ReadAsStringAsync();
            _context.PluginLogger.AppendDebug(jsonResult);
            response.EnsureSuccessStatusCode();


            return JsonConvert.DeserializeObject<UriModel>(jsonResult).apiAccessPoint + "/api/rest/v6";
        }

        internal async Task<string> GetSigningURLAsync(string agreementsId)
        {
            var apiUrl = await GetBaseUriAsync();
            var client = GetClient(apiUrl);

            string jsonResult = "";
            var statusCode = HttpStatusCode.NotFound;
            for (int i = 0; i < 5; i++)//It's too fast! The API doesn't return the signingUrl
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{apiUrl}/agreements/{agreementsId}/signingUrls");
                var response = await client.SendAsync(request);
                jsonResult = await response.Content.ReadAsStringAsync();
                _log.AppendLine($"Response status {statusCode = response.StatusCode}:").AppendLine(jsonResult);

                if (statusCode == HttpStatusCode.NotFound)                
                    Thread.Sleep(1000);                             
                else
                    break;
            }

            if (statusCode != HttpStatusCode.OK)
                throw new Exception($"Status {statusCode}");

            return JsonConvert.DeserializeObject<Models.Self.SelfSignResponse>(jsonResult).signingUrlSetInfos[0].signingUrls[0].esignUrl;
        }

        public async Task<string> SendDocumentAsync(byte[] content, string fileName)
        {
            _apiUrl = await GetBaseUriAsync();
            var client = GetClient(_apiUrl);
            var multiForm = new MultipartFormDataContent();

            var imageContent = new ByteArrayContent(content);
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

            multiForm.Add(imageContent, "File", fileName);

            var response = await client.PostAsync($"{_apiUrl}/transientDocuments", multiForm);
            var result = await response.Content.ReadAsStringAsync();
            _context.PluginLogger.AppendDebug(result);
            response.EnsureSuccessStatusCode();          
            _log.AppendLine("OperationId: " + result);
            return JsonConvert.DeserializeObject<Models.Send.DosumentResponseModel>(result).transientDocumentId;
        }

        public async Task<string> SendToSigAsync(string docId, string title, string msg, List<Models.Send.Participantsetsinfo> members, bool selfSign = false, string url = "")
        {
            var jsonObject = new Models.Send.SendRequest();
            jsonObject.fileInfos = new Models.Send.Fileinfo[] { new Models.Send.Fileinfo() { transientDocumentId = docId } };
            jsonObject.name = title;
            jsonObject.message = msg;
            jsonObject.signatureType = "ESIGN";
            jsonObject.state = "IN_PROCESS";                
            jsonObject.participantSetsInfo = members.ToArray();

            string sendOptions = "ALL";
            if (selfSign)
            {
                jsonObject.postSignOption = new Models.Send.Postsignoption() { redirectDelay = 1, redirectUrl = url };
                sendOptions = "NONE";
            }
            jsonObject.emailOption = new Models.Send.Emailoption()
            {
                sendOptions = new Models.Send.Sendoptions()
                {
                    initEmails = sendOptions,
                    inFlightEmails = sendOptions,
                    completionEmails = sendOptions
                }
            };

            var client = GetClient(_apiUrl);
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_apiUrl}/agreements")
            {
                Content = new StringContent(JsonConvert.SerializeObject(jsonObject, Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            }), Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            _context.PluginLogger.AppendDebug(result);
            response.EnsureSuccessStatusCode();         
            _log.AppendLine("AgreementsId:" + result);
            return JsonConvert.DeserializeObject<Models.Send.SigResponseModel>(result).id;
        }

        public async Task<string> GetStatusAsync(string docId)
        {
            var apiUrl = await GetBaseUriAsync();
            var client = GetClient(apiUrl);

            var request = new HttpRequestMessage(HttpMethod.Get, $"{apiUrl}/agreements/{docId}");

            var response = await client.SendAsync(request);          
            var result = await response.Content.ReadAsStringAsync();
            _log.AppendLine("Response:").AppendLine(result);
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<SingleStatusResponse>(result).status;
        }

        public async Task<byte[]> GetDocumentAsync(string docId)
        {
            var apiUrl = await GetBaseUriAsync();
            var client = GetClient(apiUrl);

            var request = new HttpRequestMessage(HttpMethod.Get, $"{apiUrl}/agreements/{docId}/combinedDocument");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();                

            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task SendRemindAsync(string docId, List<string> membersId)
        {
            var users = new List<string>();
            foreach(var id in membersId)
            {
                users.Add(id);
            }
            var apiUrl = await GetBaseUriAsync();
            var jsonObject = new Models.Remind.RemindRequest();
            jsonObject.status = "ACTIVE";
            jsonObject.recipientParticipantIds = users.ToArray();

            var client = GetClient(apiUrl);
            var request = new HttpRequestMessage(HttpMethod.Post, $"{apiUrl}/agreements/{docId}/reminders")
            {
                Content = new StringContent(JsonConvert.SerializeObject(jsonObject, Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            }), Encoding.UTF8, "application/json")
            };
            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            _context.PluginLogger.AppendDebug(result);
            response.EnsureSuccessStatusCode();
            _log.AppendLine("AgreementsId:" + result);
        }

        public async Task<Models.Remind.Memberinfo[]> GetMembersIdsAsync(string operationId)
        {
            var apiUrl = await GetBaseUriAsync();
            var client = GetClient(apiUrl);

            var request = new HttpRequestMessage(HttpMethod.Get, $"{apiUrl}/agreements/{operationId}/members");

            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            _log.AppendLine("Response:").AppendLine(result);
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<Models.Remind.ResponseMembersModel>(result).participantSets[0].memberInfos;            
        }

        public async Task DeleteAgreementAsync(string operationId)
        {
            var apiUrl = await GetBaseUriAsync();

            var client = GetClient(apiUrl);
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{apiUrl}/agreements/{operationId}/documents");
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _context.PluginLogger.AppendDebug(await response.Content.ReadAsStringAsync());
                response.EnsureSuccessStatusCode();
            }           
        }

        public async Task<Agreements> GetAllStatusAsync()
        {
            var apiUrl = await GetBaseUriAsync();
            var client = GetClient(apiUrl);

            var request = new HttpRequestMessage(HttpMethod.Get, $"{apiUrl}/agreements");

            var response = await client.SendAsync(request);
            var responseJson = await response.Content.ReadAsStringAsync();
            _log.AppendLine("Response:").AppendLine(responseJson);
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<Agreements>(responseJson);
        }


        private HttpClient GetClient(string url)
        {
            var proxy = ProxyProvider.TryGetProxy(url, _useProxy, _connectionsHelper);
            var client = proxy == null ? new HttpClient() : new HttpClient(new HttpClientHandler() { Proxy = proxy });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            return client;
        }
    }
}
