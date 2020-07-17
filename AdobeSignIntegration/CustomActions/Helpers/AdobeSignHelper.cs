using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System;
using WebCon.BpsExt.Signing.AdobeSign.CustomActions.Models;
using System.Threading;

namespace WebCon.BpsExt.Signing.AdobeSign.CustomActions.Helpers
{
    public class AdobeSignHelper
    {
        const string BaseUri = "https://secure.na1.echosign.com/api/rest/v6/baseUris";              
        StringBuilder _log;
        string _apiUrl;       

        public AdobeSignHelper(StringBuilder log)
        {
            ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol |
                                                   SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;           
            _log = log;            
        }

        private string GetBaseUri(string token)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var request = new HttpRequestMessage(HttpMethod.Get, BaseUri);
            var response = client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var jsonResult = response.Content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject<UriModel>(jsonResult).apiAccessPoint + "/api/rest/v6";
        }

        internal string GetSigningURL(string token, string agreementsId)
        {
            var apiUrl = GetBaseUri(token);
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string jsonResult = "";
            var statusCode = HttpStatusCode.NotFound;
            for (int i = 0; i < 5; i++)//It's too fast! The API doesn't return the signingUrl
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{apiUrl}/agreements/{agreementsId}/signingUrls");
                var response = client.SendAsync(request).Result;
                jsonResult = response.Content.ReadAsStringAsync().Result;
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

        public string SendDocument(byte[] content, string token, string fileName)
        {
            _apiUrl = GetBaseUri(token);
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var multiForm = new MultipartFormDataContent();

            var imageContent = new ByteArrayContent(content);
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

            multiForm.Add(imageContent, "File", fileName);

            var response = client.PostAsync($"{_apiUrl}/transientDocuments", multiForm).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsStringAsync().Result;
            _log.AppendLine("OperationId: " + result);
            return JsonConvert.DeserializeObject<Models.Send.DosumentResponseModel>(result).transientDocumentId;
        }

        public string SendToSig(string docId, string token, string title, string msg, List<Models.Send.Participantsetsinfo> members, bool selfSign = false, string url = "")
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

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_apiUrl}/agreements")
            {
                Content = new StringContent(JsonConvert.SerializeObject(jsonObject, Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            }), Encoding.UTF8, "application/json")
            };

            var response = client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsStringAsync().Result;
            _log.AppendLine("AgreementsId:" + result);
            return JsonConvert.DeserializeObject<Models.Send.SigResponseModel>(result).id;
        }

        public string GetStatus(string docId, string token)
        {
            var apiUrl = GetBaseUri(token);
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
          
            var request = new HttpRequestMessage(HttpMethod.Get, $"{apiUrl}/agreements/{docId}");

            var response = client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsStringAsync().Result;
            _log.AppendLine("Response:").AppendLine(result);

            return JsonConvert.DeserializeObject<SingleStatusResponse>(result).status;
        }

        public byte[] GetDocument(string docId, string token)
        {
            var apiUrl = GetBaseUri(token);
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new HttpRequestMessage(HttpMethod.Get, $"{apiUrl}/agreements/{docId}/combinedDocument");

            var response = client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();                

            return response.Content.ReadAsByteArrayAsync().Result;
        }

        public void SendRemind(string docId, string token, List<string> membersId)
        {
            var users = new List<string>();
            foreach(var id in membersId)
            {
                users.Add(id);
            }
            var apiUrl = GetBaseUri(token);
            var jsonObject = new Models.Remind.RemindRequest();
            jsonObject.status = "ACTIVE";
            jsonObject.recipientParticipantIds = users.ToArray();

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var request = new HttpRequestMessage(HttpMethod.Post, $"{apiUrl}/agreements/{docId}/reminders")
            {
                Content = new StringContent(JsonConvert.SerializeObject(jsonObject, Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            }), Encoding.UTF8, "application/json")
            };
            var response = client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsStringAsync().Result;
            _log.AppendLine("AgreementsId:" + result);
        }

        public Models.Remind.Memberinfo[] GetMembersIds(string operationId, string token)
        {
            var apiUrl = GetBaseUri(token);
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new HttpRequestMessage(HttpMethod.Get, $"{apiUrl}/agreements/{operationId}/members");

            var response = client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsStringAsync().Result;
            _log.AppendLine("Response:").AppendLine(result);

            return JsonConvert.DeserializeObject<Models.Remind.ResponseMembersModel>(result).participantSets[0].memberInfos;            
        }

        public void DeleteAgreement(string operationId, string token)
        {
            var apiUrl = GetBaseUri(token);

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);        
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{apiUrl}/agreements/{operationId}/documents");
            var response = client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
        }

        public Agreements GetAllStatus(string token)
        {
            var apiUrl = GetBaseUri(token);
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new HttpRequestMessage(HttpMethod.Get, $"{apiUrl}/agreements");

            var response = client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var responseJson = response.Content.ReadAsStringAsync().Result;
            _log.AppendLine("Response:").AppendLine(responseJson);

            return JsonConvert.DeserializeObject<Agreements>(responseJson);
        }
    }
}
