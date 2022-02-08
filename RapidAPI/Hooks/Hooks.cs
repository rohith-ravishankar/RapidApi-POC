using AventStack.ExtentReports;
using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.Reporter;
using BoDi;
using Microsoft.Extensions.Configuration;
using System;
using TechTalk.SpecFlow;

namespace RapidApi.Hooks
{
    [Binding]
    public class Hooks
    {
        private readonly IObjectContainer _objectContainer;
        private static ExtentTest _featureName;
        private static ExtentTest _scenario;
        private static ExtentReports _extent;
        private static ExtentHtmlReporter _htmlReporter;
        
        public Hooks(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeScenario]
        public void StartUp()
        {
            //Adds json and registers instance to be used throughout the execution
            IConfiguration configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .Build();
            _objectContainer.RegisterInstanceAs(configuration);  
        }

        [BeforeTestRun]
        public static void ExtentReporter()
        {
            var log = DateTime.Now.ToString().Replace(" ", "").Replace(":", "").Replace("/", "");
            _extent = new ExtentReports();
            _htmlReporter = new ExtentHtmlReporter(AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\netcoreapp3.1", "") +
            "Reports\\" + "Report - " + log.Replace("PM", "") + "\\" );  //Creates new directory for report
            _extent.AttachReporter(_htmlReporter);   //Consumes the directory for extent report creation
        }

        [BeforeFeature]
        public static void BeforeFeature()
        {
            _featureName = _extent.CreateTest<Feature>(FeatureContext.Current.FeatureInfo.Title);
            Console.WriteLine("BeforeFeature");
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            Console.WriteLine("BeforeScenario");
            _scenario = _featureName.CreateNode<Scenario>(ScenarioContext.Current.ScenarioInfo.Title);
        }

        [AfterStep]
        public void InsertReportingSteps()
        {
            //Captures the gherkin statements in report
            var stepType = ScenarioStepContext.Current.StepInfo.StepDefinitionType.ToString();
            switch (ScenarioContext.Current.TestError)
            {
                case null:
                    if (stepType == "Given")
                        _scenario.CreateNode<Given>(ScenarioStepContext.Current.StepInfo.Text);
                    else if (stepType == "When")
                        _scenario.CreateNode<When>(ScenarioStepContext.Current.StepInfo.Text);
                    else if (stepType == "Then")
                        _scenario.CreateNode<Then>(ScenarioStepContext.Current.StepInfo.Text);
                    else if (stepType == "And")
                        _scenario.CreateNode<And>(ScenarioStepContext.Current.StepInfo.Text);
                    break;
                default:
                    if (ScenarioContext.Current.TestError != null)
                    {
                        if (stepType == "Given")
                            _scenario.CreateNode<Given>(ScenarioStepContext.Current.StepInfo.Text).Fail(ScenarioContext.Current.TestError.Message);
                        else if (stepType == "When")
                            _scenario.CreateNode<When>(ScenarioStepContext.Current.StepInfo.Text).Fail(ScenarioContext.Current.TestError.Message);
                        else if (stepType == "Then")
                            _scenario.CreateNode<Then>(ScenarioStepContext.Current.StepInfo.Text).Fail(ScenarioContext.Current.TestError.Message);
                        else if (stepType == "And")
                            _scenario.CreateNode<And>(ScenarioStepContext.Current.StepInfo.Text).Fail(ScenarioContext.Current.TestError.Message);
                    }
                    break;
            }
        }

        [AfterTestRun]
        public static void ExtentFlush()
        {
            _extent.Flush();   //Creates and adds the file in the directory from ExtentReporter()
        }

    }
}
