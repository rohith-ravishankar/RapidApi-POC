Feature: RapidApi Test Feature

Scenario: Validate getLatestCountryDataByCode endpoint
	Given I have a request with 'headers'
	| parameter       | value  |
	| x-rapidapi-host |        |
	| x-rapidapi-key  |        |
	And I have a request with 'queryParameters'
	| parameter | value    |
	| code      | <code>   |
	| format    | <format> |
	When I call '/country/code' endpoint
	Then I should get '<status>' response
	And I validate '<noOfItems>' and '<code>' in '<format>'
	Examples:
	| code | format  | status | noOfItems |
	| it   |         | 200    | 1         |
	| siin |         | 200    | 0         |
	|      |         | 200    | 0         |
	| it   | json    | 200    | 1         |
	| it   | xml     | 200    | 1         |
	| it   | invalid | 200    | 1         |

Scenario: Validate response with invalid headers
	Given I have a request with 'headers'
	| parameter       | value  |
	| x-rapidapi-host | <host> |
	| x-rapidapi-key  | <key>  |
	And I have a request with 'queryParameters'
	| parameter | value    |
	| code      | it       |
	| format    | json	   |
	When I call '/country/code' endpoint
	Then I should get '<status>' response
	Examples:
	| host        | key                  | status |
	| invalidHost |                      | 400    |
	|             | invalidAuthorization | 403    |

Scenario: Validate response without query parameters
	Given I have a request with 'headers'
	| parameter       | value  |
	| x-rapidapi-host |        |
	| x-rapidapi-key  |        |
	When I call '/country/code' endpoint
	Then I should get '400' response

Scenario: Validate response without headers
	Given I have a request with 'queryParameters'
	| parameter | value |
	| code      | it    |
	| format    | json  |
	When I call '/country/code' endpoint
	Then I should get '401' response

#Avoid running because of request limitation. Execution time is greater than 20mins
Scenario: Validate stats for most affected countries
	Given I have a request with 'headers'
	| parameter       | value  |
	| x-rapidapi-host |        |
	| x-rapidapi-key  |        |
	And I have a request with 'queryParameters'
	| parameter | value |
	| code      | all   |
	| format    | json  |
	When I call '/country/code' and get most cases
	| condition |
	| confirmed |
	| recovered |
	| critical  |
	| deaths    |


