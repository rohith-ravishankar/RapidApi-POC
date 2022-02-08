using Newtonsoft.Json;
using NUnit.Framework;
using RapidAPI.Utils;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace RapidAPI.Resources
{
    public class RapidApiResource
    {
        private readonly XmlDocument _xmlResponse;

        public RapidApiResource()
        {
            this._xmlResponse = new XmlDocument();
        }

        public RestResponse GetRapidApiResponse(string host, Dictionary<string, string> headers, Dictionary<string, string> queryParameters)
        {
            //Takes header and query parameters, builds the request and returns the response
            var client = new RestClient("https://" + host);
            var request = new RestRequest();
            if(headers!=null)
                request.AddHeaders(headers);
            if (queryParameters != null)
            {
                foreach (var queryParameter in queryParameters.Keys)
                {
                    request.AddQueryParameter(queryParameter, queryParameters[queryParameter]);
                }
            }
            Task<RestResponse> response = client.ExecuteGetAsync(request);
            return response.Result;
        }

        public XmlNode XmlResponse(int numberOfItems, string code, RestResponse response)
        {
            //XML validation of the structure and content within
            _xmlResponse.LoadXml(response.Content);
            XmlNodeList xmlNodes = _xmlResponse.DocumentElement.ChildNodes;  //Child nodes referred for tags within tags
            Assert.True(xmlNodes.Count.Equals(numberOfItems));
            foreach (XmlNode xmlNode in xmlNodes)
            {
                var items = xmlNode.ChildNodes;
                foreach(XmlNode item in items)
                {
                    if (item.Name.Equals("code"))
                    {
                        Assert.True(item.InnerText.ToLower().Equals(code));
                    }
                }
                return xmlNode;
            }
            return null;
        }

        public Model JsonResponse(int numberOfItems, string code, RestResponse response)
        {
            //Json validation of the structure and content within
            Regex regex = new Regex(@"^-?[0-9][0-9,\.]+$");
            var responseContent = JsonConvert.DeserializeObject<List<Model>>(response.Content);
            if ((int)response.StatusCode == 200)
            {
                Assert.AreEqual(responseContent.Count, numberOfItems);   //Validates the response for correct count - e.g: only one country data
                if (responseContent.Count > 0)
                {
                    Assert.AreEqual(responseContent[0].code.ToLower(), code);
                    Assert.True(regex.IsMatch(responseContent[0].confirmed));
                    Assert.True(regex.IsMatch(responseContent[0].recovered));  //Validates the data in correct format
                    Assert.True(regex.IsMatch(responseContent[0].critical));
                    Assert.True(regex.IsMatch(responseContent[0].deaths));
                    return responseContent[0];
                }
            }
            return null;    //In case of zero items in the response - return null
        }
    }
}
