using Godot;
using System;
using BCrypt.Net;
using System.Text.Json;

public partial class RegisterScene : Control
{
	[Export] private HttpRequest _httpRequest;
	[Export] private LineEdit _usernameLineEdit;
	[Export] private LineEdit _emailLineEdit;
	[Export] private LineEdit _passwordLineEdit;
	[Export] private Label _usernameErrorLabel;
	[Export] private Label _emailErrorLabel;
	[Export] private Label _passwordErrorLabel;
	[Export] private Button _registerButton;
	[Export] private LinkButton _signInLinkButton;
	[Export] private Control _loadingComponent;
	private bool _isRequestSend = false;

	public override void _Ready()
	{
		_httpRequest.RequestCompleted += OnHttpRequestCompleted;
		_registerButton.Pressed += OnRegisterButtonPressed;
		_signInLinkButton.Pressed += OnSignInLinkButtonPressed;
		_loadingComponent.Visible = false;
	}

	private string GetUsername()
	{
		return _usernameLineEdit.Text;
	}

	private string GetEmail()
	{
		return _emailLineEdit.Text;
	}

	private string GetPassword()
	{
		return _passwordLineEdit.Text;
	}

	private ValidationResult ValidateUsername(string username)
	{
		if (string.IsNullOrEmpty(username))
		{
			return ValidationResult.Fail("Username cannot be empty");
		}
		return ValidationResult.Success();
	}

	private ValidationResult ValidateEmail(string email)
	{
		if (string.IsNullOrEmpty(email))
		{
			return ValidationResult.Fail("Email cannot be empty");
		}

		if (!email.Contains("@") || !email.Contains("."))
		{
			return ValidationResult.Fail("Invalid email format");
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

	private void SubmitRegistrationToServer(string username, string email, string password)
	{
		if (_isRequestSend) { return; }
		_loadingComponent.Visible = true;
		string registerUrl = ApiClient.Instance.BuildUrl("users/register");

		// prepare headers for the request
		var headers = new string[] { "Content-Type: application/json" };

		// prepare body for the request
		var data = new Godot.Collections.Dictionary
		{
			{"username", username},
			{"password", password},
			{"email", email}
		};
		string body = Json.Stringify(data);

		// submit request
		_httpRequest.Request(registerUrl, headers, HttpClient.Method.Post, body);
		_isRequestSend = true;
	}

	private void OnHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		_isRequestSend = false;
		_loadingComponent.Visible = false;

		if (responseCode == 201)
		{
			string jsonResponse = System.Text.Encoding.UTF8.GetString(body);

			UserDataManager.Instance.CurrentUser = JsonSerializer.Deserialize<UserData>(jsonResponse);

			SceneManager.Instance.ChangeToMainMenuScene();
		}
		else if (responseCode >= 400)
		{
			string jsonResponse = System.Text.Encoding.UTF8.GetString(body);

			if (jsonResponse.Contains("email", StringComparison.OrdinalIgnoreCase))
			{
				_emailErrorLabel.Text = jsonResponse;
				_emailErrorLabel.Visible = true;
			}
			if (jsonResponse.Contains("username", StringComparison.OrdinalIgnoreCase))
			{
				_usernameErrorLabel.Text = jsonResponse;
				_usernameErrorLabel.Visible = true;
			}
		}
	}

	private void OnRegisterButtonPressed()
	{
		string username = GetUsername();
		string email = GetEmail();
		string password = GetPassword();

		// reset error labels
		_usernameErrorLabel.Text = string.Empty;
		_emailErrorLabel.Text = string.Empty;
		_passwordErrorLabel.Text = string.Empty;
		_usernameErrorLabel.Visible = false;
		_emailErrorLabel.Visible = false;
		_passwordErrorLabel.Visible = false;

		var usernameValidation = ValidateUsername(username);
		var emailValidation = ValidateEmail(email);
		var passwordValidation = ValidatePassword(password);

		if (!usernameValidation.IsValid)
		{
			_usernameErrorLabel.Visible = true;
			_usernameErrorLabel.Text = usernameValidation.ErrorMessage;
			return;
		}

		if (!emailValidation.IsValid)
		{
			_emailErrorLabel.Visible = true;
			_emailErrorLabel.Text = emailValidation.ErrorMessage;
			return;
		}

		if (!passwordValidation.IsValid)
		{
			_passwordErrorLabel.Visible = true;
			_passwordErrorLabel.Text = passwordValidation.ErrorMessage;
			return;
		}

		SubmitRegistrationToServer(username, email, password);
	}

	private void OnSignInLinkButtonPressed()
	{
		SceneManager.Instance.ChangeToLoginScene();
	}
}
