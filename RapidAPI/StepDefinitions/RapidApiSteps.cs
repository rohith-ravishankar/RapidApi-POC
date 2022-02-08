using AventStack.ExtentReports;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;
using RapidAPI.Resources;
using RapidAPI.Utils;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace RapidAPI.StepDefinitions
{
    [Binding]
    public class RapidApiSteps
    {
        private readonly TestConfig _test;
        private readonly ScenarioContext _scenarioContext;
        private readonly RapidApiResource _apiResource;
        private readonly StatsResource _statsResource;
        private readonly RequestBuilder _requestBuilder;
        private readonly ExtentTest _feature;

        public RapidApiSteps(IConfiguration configuration, ScenarioContext scenarioContext)
        {
            _test = configuration.GetSection("RapidApi").Get<TestConfig>();  //Provides data from json
            _scenarioContext = scenarioContext;
            _apiResource = new RapidApiResource();
            _statsResource = new StatsResource();
            _requestBuilder = new RequestBuilder();
        }

        [Given(@"I have a request with '(.*)'")]
        public void GivenIHaveARequestWith(string parameterType, Table parameterTable)
        {
            //Switcher for building headers and query parameters
            var requestParameters = parameterTable.CreateSet<RequestParams>();
            switch (parameterType)
            {
                case "headers":
                    Dictionary<string, string> headers = _requestBuilder.AddHeaders(requestParameters, _test);
                    _scenarioContext.Add("Headers", headers);
                    break;
                case "queryParameters":
                    Dictionary<string, string> queryParameters = _requestBuilder.AddQueryParameters(requestParameters, _test);
                    _scenarioContext.Add("QueryParameters", queryParameters);
                    break;
                default:
                    break;
            }
        }

        [When(@"I call '(.*)' endpoint")]
        public void WhenICallTheEndpoint(string endpoint)
        {
            //Calls endpoint and returns response
            Dictionary<string, string> headers = _scenarioContext.TryGetValue("Headers", out headers) ? headers : null;
            Dictionary<string, string> queryParameters = _scenarioContext.TryGetValue("QueryParameters", out queryParameters) ? queryParameters : null;
            RestResponse response = _apiResource.GetRapidApiResponse(_test.host + endpoint, headers, queryParameters);
            _scenarioContext.Add("Response", response);
            Thread.Sleep(2000);   //Added to avoid 429 status during execution
        }

        [Then(@"I should get '(.*)' response")]
        public void ThenIShouldGetAResponse(int statusCode)
        {
            RestResponse response = _scenarioContext.TryGetValue("Response", out response) ? response : null;
            Assert.AreEqual(statusCode, (int)response.StatusCode);
        }

        [Then(@"I validate '(.*)' and '(.*)' in '(.*)'")]
        public void ThenResponseShouldHave(int numberOfItems, string code, string format)
        {
            //Switcher for JSON/XML validation
            Model jsonContent;
            XmlNode xmlContent;
            RestResponse response = _scenarioContext.TryGetValue("Response", out response) ? response : null;
            switch (format)
            {
                case "json":
                    jsonContent = _apiResource.JsonResponse(numberOfItems, code, response);
                    _scenarioContext.Add("ResponseContent", jsonContent);
                    break;
                case "xml":
                    xmlContent = _apiResource.XmlResponse(numberOfItems, code, response);
                    _scenarioContext.Add("ResponseContent", xmlContent);
                    break;
                case "":
                    jsonContent = _apiResource.JsonResponse(numberOfItems, code, response);
                    _scenarioContext.Add("ResponseContent", jsonContent);
                    break;
            }
        }

        [When(@"I call '(.*)' and get most cases")]
        public void ThenICallAndGetAllCountryInfo(string endpoint, Table conditionTable)
        {
            //Additive to get data for most affected countries - this can be refined
            var conditions = conditionTable.Rows.Select(row => row["condition"]);
            Dictionary<string, string> headers = _scenarioContext.TryGetValue("Headers", out headers) ? headers : null;
            foreach (var condition in conditions)
            {
                //for each ISO code, store the max values, compare and return highest value
                foreach (var isoCode in _statsResource.GetListOfIsoCodes())
                {
                    Dictionary<string, string> queryParameters = _scenarioContext.TryGetValue("QueryParameters", out queryParameters) ? queryParameters : null;
                    queryParameters.Add("code", isoCode);
                    RestResponse response = _apiResource.GetRapidApiResponse(_test.host + endpoint, headers, queryParameters);
                    var responseContent = JsonConvert.DeserializeObject<List<Model>>(response.Content);
                    if (responseContent.Count > 0)
                    {
                        _statsResource.FindMostCases(condition, responseContent[0]);
                    }
                    queryParameters.Remove("code");
                    Thread.Sleep(1500); //Added to avoid 429 status during execution
                }
                Console.WriteLine(_statsResource.mostCasesCountry + " has the most number of " + condition + " cases with a value of " + _statsResource.mostCasesValue);
                if (!condition.Equals("confirmed"))
                    Console.WriteLine(_statsResource.highestPercentCountry + " has the highest " + condition + " percentage with a value of " + _statsResource.highestPercentValue + "%");
                _statsResource.ResetMaxValues();
            }
            
        }
    }
}
