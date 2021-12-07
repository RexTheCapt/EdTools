using Newtonsoft.Json.Linq;

using RestSharp;

using System;
using System.Collections.Generic;
using System.Threading;

// Stolen from https://github.com/pulganosaure/ED-NeutronRouter/tree/888c16dc527e66cd770766151512bc8395bd9213

#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
namespace EDNeutronRouterPlugin
{
    public class NeutronRouterAPI
    {
        public static JToken GetNewRoute(string currentSystem, string SystemTarget, decimal jumpDistance, int efficiency)
        {
            JObject routeResponse;
            var Route = PlotRoute(currentSystem, SystemTarget, jumpDistance, efficiency).Content;
            if (Route == null || Route == "")
            {
                throw new UnableToCallSpanshApiException();
            }

            routeResponse = JObject.Parse(Route);

            if (routeResponse == null)
            {
                throw new RouteIsNullException();
            }
            else if (routeResponse["error"] != null)
            {
                throw new Exception(routeResponse["error"]?.ToString());
            }
            else
            {
                if (routeResponse == null)
                    throw new RouteIsNullException();
                if (routeResponse["job"] == null)
                    throw new RouteResponseJobIsNullException();

                var job = routeResponse["job"].ToString();
                JObject routeResult = GetRouteResults(job);
                while (routeResult["status"] != null && routeResult["status"].ToString() == "queued" && routeResult["error"] == null)
                {
                    Thread.Sleep(1000);
                    routeResult = GetRouteResults(job);
                }
                if (routeResult["error"] != null)
                {
                    throw new Exception(routeResult["error"]?.ToString());
                }
                if (routeResult["result"] == null)
                    throw new RouteResultIsNullException();

                return routeResult["result"];
            }
        }

        public static List<EDSystem> GetSystemList(JToken routeResult)
        {
            if (routeResult == null)
                throw new RouteResultIsNullException();
            if (routeResult["system_jumps"] == null)
                throw new RouteResultSystemJumpsIsNullException();

            JArray Systems = routeResult["system_jumps"].ToObject<JArray>();

            List<EDSystem> SystemList = new List<EDSystem>();

            if (Systems == null)
                throw new SystemIsNullException();
            for (int i = 0; i < Systems.Count; i++)
            {
                var System = Systems[i];
                EDSystem test = new(System["distance_left"].ToObject<double>(), Systems[i]["jumps"].ToObject<int>(), Systems[i]["neutron_star"].ToObject<bool>(), Systems[i]["system"].ToObject<string>());
                SystemList.Add(test);
            }

            return SystemList;
        }

        //get the job id from the spansh API.
        public static IRestResponse PlotRoute(string Position, string Destination, decimal range, int Efficiency)
        {

            var client = new RestClient("https://spansh.co.uk/api/");
            var request = new RestRequest("route");
            request.AddParameter("efficiency", 60)
                .AddParameter("range", range.ToString().Replace(",", "."))
                .AddParameter("from", Position)
                .AddParameter("to", Destination);
            var response = client.Get(request);
            return response;


        }
        //get the system List from the the spansh API
        public static JObject GetRouteResults(string job)
        {
            var client = new RestClient("https://spansh.co.uk/api/");
            var Jobrequest = new RestRequest("results/" + job);

            var response = client.Get(Jobrequest);
            return JObject.Parse(response.Content);
        }
    }
}
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8602 // Dereference of a possibly null reference.