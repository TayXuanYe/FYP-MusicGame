using Godot;
using System;

public partial class LoginScreen : Control
{
	private HttpRequest _httpRequest;
	private LineEdit _usernameLineEdit;
	private LineEdit _passwordLineEdit;
	private Label _usernameErrorLabel;
	private Label _passwordErrorLabel;
	
	public override void _Ready()
	{
		_httpRequest = GetNode<HttpRequest>("HTTPRequest");
		_httpRequest.RequestCompleted += OnHttpRequestCompleted;
		
		Button loginButton = GetNode<Button>("LoginButton");
		loginButton.Pressed += OnLoginButtonPressed;
		
		_usernameLineEdit = GetNode<LineEdit>("UsernameLineEdit");
		_passwordLineEdit = GetNode<LineEdit>("PasswordLineEdit");
		_usernameErrorLabel = GetNode<Label>("UsernameErrorLabel");
		_passwordErrorLabel = GetNode<Label>("PasswordErrorLabel");
	}
	
	private string GetUsername()
	{
		var username = _usernameLineEdit;
		return username.Text;
	}
	
	private string GetPassword()
	{
		var password = _passwordLineEdit;
		return password.Text;
	}
	
	private ValidationResult ValidateUsername(string username)
	{
		if (string.IsNullOrEmpty(username))
		{
			return ValidationResult.Fail("Username cannot be empty");
		}
		
		return ValidationResult.Success();
	}
	
	private ValidationResult ValidatePassword(string password)
	{
		if (string.IsNullOrEmpty(password))
		{
			return ValidationResult.Fail("Password cannot be empty");
		}
		
		if (password.Length < 8 || password.Length > 32)
		{
			return ValidationResult.Fail("Length of password must be between 8 to 32 characters");
		}
		
		return ValidationResult.Success();
	}
	
	private void SubmitLoginToServer(string username, string password)
	{
		// temp login url haven‘t build backend
		string loginUrl = "https://your-server-api.com/login";
		
		// prepare headers for the request
		var headers = new string[] { "Content-Type: application/json" };
		
		// prepare body for the request
		var data = new Godot.Collections.Dictionary
		{
			{"username", username},
			{"password", password}
		};
		string body = Json.Stringify(data);
		
		// submit request
		_httpRequest.Request(loginUrl, headers, HttpClient.Method.Post, body);
	}
	
	private void OnHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		// handle the response from the server
	}
	
	private void OnLoginButtonPressed()
	{
		string username = GetUsername();
		string password = GetPassword();
		
		Label usernameErrorLabel = _usernameErrorLabel;
		Label passwordErrorLabel = _passwordErrorLabel;
		
		//reset error message field error message and visible state
		usernameErrorLabel.Text = "";
		passwordErrorLabel.Text = "";
		usernameErrorLabel.Visible = false;
		passwordErrorLabel.Visible = false;
		
		
		//verify username and password are valid
		var usernameValidateResult = ValidateUsername(username);
		var passwordValidateResult = ValidatePassword(password);
		if (!usernameValidateResult.IsValid)
		{
			usernameErrorLabel.Visible = true;
			usernameErrorLabel.Text = usernameValidateResult.ErrorMessage;
			return;
		}
		if(!passwordValidateResult.IsValid)
		{
			passwordErrorLabel.Visible = true;
			passwordErrorLabel.Text = passwordValidateResult.ErrorMessage;
			return;
		}
		// future enhance
		string hashPassword = password;
		SubmitLoginToServer(username, hashPassword);
	}
}
