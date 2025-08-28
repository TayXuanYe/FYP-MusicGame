using Godot;
using System;
using BCrypt.Net;

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


	public override void _Ready()
	{
		_httpRequest.RequestCompleted += OnHttpRequestCompleted;
		_registerButton.Pressed += OnRegisterButtonPressed;
		_signInLinkButton.Pressed += OnSignInLinkButtonPressed;
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

	private bool SubmitEmailUniqueValidationToServer(string email)
	{
		// future enhance
		return true;
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

		if (!SubmitEmailUniqueValidationToServer(email))
		{
			return ValidationResult.Fail("Email already exists");
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

		string hashPassword = BCrypt.Net.BCrypt.HashPassword(password);
		SubmitRegistrationToServer(username, email, hashPassword);
	}

	private void OnSignInLinkButtonPressed()
	{
		SceneManager.Instance.ChangeToLoginScene();
	}
}
