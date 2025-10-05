using Godot;
using System;
using BCrypt.Net;
using System.Text.Json;
public partial class LoginScene : Control
{
	[Export] private HttpRequest _httpRequest;
	[Export] private LineEdit _usernameLineEdit;
	[Export] private LineEdit _passwordLineEdit;
	[Export] private Label _usernameErrorLabel;
	[Export] private Label _passwordErrorLabel;
	[Export] private Button _loginButton;
	[Export] private LinkButton _createAccountLinkButton;
	[Export] private Control _loadingComponent;

	public override void _Ready()
	{
		_httpRequest.RequestCompleted += OnHttpRequestCompleted;
		_loginButton.Pressed += OnLoginButtonPressed;
		_createAccountLinkButton.Pressed += OnCreateAccountLinkButtonPressed;
		_loadingComponent.Visible = false;
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

	private bool _isRequestSend = false;
	private void SubmitLoginToServer(string username, string password)
	{
		if (_isRequestSend) { return; }
		_loadingComponent.Visible = true;
		_isRequestSend = true;
		string loginUrl = ApiClient.Instance.BuildUrl("users/login");

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
		_isRequestSend = false;
		_loadingComponent.Visible = false;

		if (responseCode == 200)
		{
			string jsonResponse = System.Text.Encoding.UTF8.GetString(body);
			GD.Print($"Success Response body: {jsonResponse}");

			UserDataManager.Instance.CurrentUser = JsonSerializer.Deserialize<UserData>(jsonResponse);

			SceneManager.Instance.ChangeToMainMenuScene();
		}
		else if (responseCode >= 400)
		{
			string jsonResponse = System.Text.Encoding.UTF8.GetString(body);

			if (jsonResponse.Contains("password", StringComparison.OrdinalIgnoreCase))
			{
				_passwordErrorLabel.Visible = true;
				_passwordErrorLabel.Text = jsonResponse;
			}
			if (jsonResponse.Contains("User", StringComparison.OrdinalIgnoreCase))
			{
				_usernameErrorLabel.Visible = true;
				_usernameErrorLabel.Text = jsonResponse;
			}
		}
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
		bool isValid = true;
		
		//verify username and password are valid
		var usernameValidateResult = ValidateUsername(username);
		var passwordValidateResult = ValidatePassword(password);
		if (!usernameValidateResult.IsValid)
		{
			usernameErrorLabel.Visible = true;
			usernameErrorLabel.Text = usernameValidateResult.ErrorMessage;
			isValid = false;
		}
		if(!passwordValidateResult.IsValid)
		{
			passwordErrorLabel.Visible = true;
			passwordErrorLabel.Text = passwordValidateResult.ErrorMessage;
			isValid = false;
		}
		if (!isValid) { return; }
		
		SubmitLoginToServer(username, password);
	}

	private void OnCreateAccountLinkButtonPressed()
	{
		SceneManager.Instance.ChangeToRegisterScene();
	}
}
