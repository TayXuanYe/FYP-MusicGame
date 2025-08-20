using System;

public class ValidationResult
{
	public bool IsValid { get; set; }
	public string ErrorMessage { get; set; }
	
	private ValidationResult(bool isValid, string errorMessage = "")
	{
		this.IsValid = isValid;
		this.ErrorMessage = errorMessage;
	}
	
	public static ValidationResult Success()
	{
		return new ValidationResult(true);
	}
	
	public static ValidationResult Fail(string errorMessage)
	{
		return new ValidationResult(false, errorMessage);
	}
	
}
