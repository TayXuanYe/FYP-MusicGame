using Godot;
using System;
using BCrypt.Net;

public partial class RegisterScreen : Control
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
}
