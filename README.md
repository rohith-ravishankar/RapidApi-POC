# RapidApi-POC

RapidApi - getLatestCountryDataByCode endpoint validation

Code to execute, validate and induce error responses. This has been achieved by varying the inputs, authorization etc.

Framework - C# RestSharp
•	Feature Files – Translates all the test cases in Gherkin format.
•	Hooks – Acts as an entry point to the execution. Responsible for loading data files, setting up reporter prior to execution.
•	Resources – The backbone of the execution. Builds request, sends request, receives response, validates data in a classified form.
•	StepDefinitions – Converts the Gherkin file into steps to execute. Initiates and calls all the resources for different validations.
•	Utils – Contains Data Models to be used throughout the execution.
•	AppSettings.json – Json file to be initiated at the start of execution.
•	Reports – Will be created after execution in html format. Will detail out the passed and failed tests along with the failure log.
