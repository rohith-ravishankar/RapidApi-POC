using RapidAPI.Utils;
using System.Collections.Generic;

namespace RapidAPI.Resources
{
    public class RequestBuilder
    {
        public Dictionary<string, string> AddHeaders(IEnumerable<RequestParams> requestParameters, TestConfig test)
        {
            //Creates, builds and returns headers
            Dictionary<string, string> headers = new Dictionary<string, string>();
            foreach (var requestParameter in requestParameters)
            {
                //Empty string represent header to be taken from json
                if (requestParameter.value == "" && requestParameter.parameter.Contains("host"))    
                    headers.Add(requestParameter.parameter, test.host);
                else if (requestParameter.value == "" && requestParameter.parameter.Contains("key"))
                    headers.Add(requestParameter.parameter, test.authorization);
                else
                    headers.Add(requestParameter.parameter, requestParameter.value);
            }
            return headers;
        }

        public Dictionary<string, string> AddQueryParameters(IEnumerable<RequestParams> requestParameters, TestConfig test)
        {
            //Creates, builds and returns query parameters
            Dictionary<string, string> queryParameters = new Dictionary<string, string>();
            foreach (var requestParameter in requestParameters)
            {
                if (!requestParameter.value.Equals("all"))    //Identify query params for all countries, the code=countryCode need not be built
                    queryParameters.Add(requestParameter.parameter, requestParameter.value);
            }
            return queryParameters;
        }
    }
}
