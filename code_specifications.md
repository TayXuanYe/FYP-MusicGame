# Naming convention
## Variable / Field
1. All of the naming for variable must be meaningful.
2. All of the private variable naming must using small camelCase
3. All of the public variable naming must using big camelCase
4. Avoid using public fields and use public properties instead.
5. Private fields of a class need to be preceded by an underscore.

## Properties
1. All of the properties must be meaningful and using big camelCase.

## Function
1.All of the function naming must be meaningful and using big camelCase.

2.Depending on the function, the function name begins with the following fields shown in the table. 

|Field|Function|
|:------:|:------|
|Get|Indicates that the main purpose of this function is to return a value|
|On|This function is not called by you actively, but the system (or framework) will automatically call it when a specific event occurs.|
|Validate|This function is used for justify the condition is valid or not and return an result object|

3. A function should not have more than one function or task. (Comply with the Single Responsibility Principle)

