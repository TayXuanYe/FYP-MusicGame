using Godot;
using System;
using BCrypt.Net;

public partial class LoginScreen : Control
{
	[Export] private HttpRequest _httpRequest;
	[Export] private LineEdit _usernameLineEdit;
	[Export] private LineEdit _passwordLineEdit;
	[Export] private Label _usernameErrorLabel;
	[Export] private Label _passwordErrorLabel;
	[Export] private Button _loginButton;
	[Export] private LinkButton _createAccountLinkButton;
	
	public override void _Ready()
	{
		_httpRequest.RequestCompleted += OnHttpRequestCompleted;
		_loginButton.Pressed += OnLoginButtonPressed;
		_createAccountLinkButton.Pressed += OnCreateAccountLinkButtonPressed;
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
		
		string hashPassword = BCrypt.Net.BCrypt.HashPassword(password);;
		SubmitLoginToServer(username, hashPassword);
	}

	private void OnCreateAccountLinkButtonPressed()
	{
		SceneManager.Instance.ChangeToRegisterScreen();
	}
}
