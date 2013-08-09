/* Copyright 2013 crossvertise GmbH

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace PlacetelApiClient
{
    public class PlacetelClient
    {
        private static DateTime? _lastCallTimeUtc;
        private static TimeSpan? _minCallInterval;

        /// <summary>
        /// The internal RestClient. Should not be modified directly but can be used for debug purposes. 
        /// </summary>
        public RestClient RestClient { get; private set; }

        /// <summary>
        /// Creates a new instance of the PlacetelClient.
        /// </summary>
        /// <param name="apiKey">The API key from Placetel. You find it in your Placetel account under 'Stammdaten'
        ///  > 'Einstellungen' > 'API'</param>
        /// <param name="baseUrl">the endpoint of the placetel api. The default value is 'https://api.placetel.de/api'</param>
        /// <param name="minCallInterval">The minimum call interval in milliseconds. The default is 1000ms.</param>
        public PlacetelClient(string apiKey, string baseUrl = "https://api.placetel.de/api", int? minCallInterval = 3500)
        {
            RestClient = new RestClient(baseUrl);
            RestClient.AddDefaultParameter("api_key", apiKey);
            _minCallInterval = minCallInterval.HasValue ? new TimeSpan(0, 0, 0, 0, minCallInterval.Value) : (TimeSpan?)null;
        }

        private IRestResponse<T> Execute<T>(IRestRequest request) where T : new()
        {
            if (_minCallInterval.HasValue && _lastCallTimeUtc.HasValue && _lastCallTimeUtc.Value.Add(_minCallInterval.Value) > DateTime.UtcNow)
            {
                //Calculate the time we still need to wait
                var timeout = _minCallInterval.Value - (DateTime.UtcNow - _lastCallTimeUtc.Value);

                //Wait for that period
                System.Threading.Thread.Sleep(timeout);

                
            }
            var result = RestClient.Execute<T>(request);

            //Update the time of the last call
            _lastCallTimeUtc = DateTime.UtcNow;

            return result;

        }

        public bool Test()
        {
            var request = new RestRequest("test.json", Method.POST);
            var response = Execute<ResultBase>(request);
            return response.Data.Success;
        }

        public List<Number> GetNumbers()
        {
            var request = new RestRequest("getNumbers.json", Method.POST);
            var response = Execute<List<Number>>(request);
            return response.Data;
        }

        public List<SipUser> GetVoipUsers()
        {
            var request = new RestRequest("getVoIPUsers.json", Method.POST);
            var response = Execute<List<SipUser>>(request);
            return response.Data;
        }

        public List<Prompt> GetPrompts()
        {
            var request = new RestRequest("getPrompts.json", Method.POST);
            var response = Execute<List<Prompt>>(request);
            return response.Data;
        }

        public List<Contact> GetContacts()
        {
            var request = new RestRequest("getContacts.json", Method.POST);
            var response = Execute<List<Contact>>(request);
            return response.Data;
        }

        public List<Call> GetIncomingCallsByDay(int year, int month, int day)
        {
            var request = new RestRequest("getIncomingCallsByDay.json", Method.POST);
            request.AddParameter("year", year);
            request.AddParameter("month", month);
            request.AddParameter("day", day);

            var response = Execute<List<Call>>(request);
            return response.Data;
        }

        public List<CallDetailRecord> GetCdrsByDay(int year, int month, int day)
        {
            var request = new RestRequest("getCDRsByDay.json", Method.POST);
            request.AddParameter("year", year);
            request.AddParameter("month", month);
            request.AddParameter("day", day);

            var response = Execute<List<CallDetailRecord>>(request);
            return response.Data;
        }

        public List<RoutingPlan> GetRoutingPlans()
        {
            var request = new RestRequest("getRoutingPlans.json", Method.POST);
            var response = Execute<List<RoutingPlan>>(request);
            return response.Data;
        }

        public List<RoutingSettings> GetRouting(string number)
        {
            var request = new RestRequest("getRouting.json", Method.POST);
            request.AddParameter("number", number);
            var response = Execute<List<RoutingSettings>>(request);
            return response.Data;
        }

        public bool InitiateCall(string sipUid, string target)
        {
            var request = new RestRequest("initiateCall.json", Method.POST);
            request.AddParameter("sipuid", sipUid);
            request.AddParameter("target", target);
            var response = Execute<ResultBase>(request);
            return response.Data.Success;
        }

        public bool SetRoutingCallForward(string number, RoutingSettings routingSettings)
        {
            var request = new RestRequest("setRouting_callForward.json", Method.POST);
            request.AddParameter("number", number);

            request.AddParameter("routing", routingSettings.Routing);
            request.AddParameter("routing_settings", routingSettings.Routing_Settings);
            request.AddParameter("voice_prompt_id", routingSettings.VoicePromptId);
            request.AddParameter("forward_voiceprompt_id", routingSettings.ForwardVoicePromptId);
            request.AddParameter("target1", routingSettings.Target1);
            request.AddParameter("target2", routingSettings.Target2);
            request.AddParameter("target3", routingSettings.Target3);
            request.AddParameter("target4", routingSettings.Target4);
            request.AddParameter("target5", routingSettings.Target5);
            request.AddParameter("ringing_time1", routingSettings.RingingTime1);
            request.AddParameter("ringing_time2", routingSettings.RingingTime2);
            request.AddParameter("ringing_time3", routingSettings.RingingTime3);
            request.AddParameter("ringing_time4", routingSettings.RingingTime4);
            request.AddParameter("ringing_time5", routingSettings.RingingTime5);
            request.AddParameter("routing_plan", routingSettings.RoutingPlan);


            var response = Execute<ResultBase>(request);
            return response.Data.Success;
        }

        public bool SetRoutingRoutingPlan(string number, int routingPlanId)
        {
            var request = new RestRequest("setRouting_RoutingPlan.json", Method.POST);
            request.AddParameter("number", number);
            request.AddParameter("routing_plan", routingPlanId);

            var response = Execute<ResultBase>(request);
            return response.Data.Success;
        }
    }
}
