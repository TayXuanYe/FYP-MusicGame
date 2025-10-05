using Godot;
using System;
using System.Text.Json;

public partial class ReportBugScene : Control
{
	[Export] private Button _backToHomeButton;
	[Export] private LineEdit _bugTitleLineEdit;
	[Export] private Label _bugTitleErrorLabel;
	[Export] private TextEdit _bugDescriptionTextEdit;
	[Export] private Label _bugDescriptionErrorLabel;
	[Export] private TextEdit _stepsToReproduceTextEdit;
	[Export] private Label _stepsToReproduceErrorLabel;
	[Export] private Button _submitButton;
	[Export] private HttpRequest _httpRequest;
	[Export] private Control _popupMessageComponentInstance;
	[Export] private Control _loadingComponent;
	private MessageComponent _popupMessageComponentScript;

	public override void _Ready()
	{
		_httpRequest.RequestCompleted += OnHttpRequestCompleted;
		_backToHomeButton.Pressed += OnBackToHomeButtonPressed;
		_submitButton.Pressed += OnSubmitButtonPressed;
		_popupMessageComponentInstance.Visible = false;
		_loadingComponent.Visible = false;

		_popupMessageComponentScript = (MessageComponent)_popupMessageComponentInstance;
		_popupMessageComponentScript.ClickButton.Pressed += OnBackToHomeButtonPressed;
	}

	private void OnBackToHomeButtonPressed()
	{
		SceneManager.Instance.ChangeToMainMenuScene();
	}

	private void OnSubmitButtonPressed()
	{
		bool isValid = ValidateForm();
		if (isValid)
		{
			SubmitReportToServer(_bugTitleLineEdit.Text, _bugDescriptionTextEdit.Text, _stepsToReproduceTextEdit.Text);
		}
	}

	private bool ValidateForm()
	{
		bool isValid = true;

		// Validate Bug Title
		if (string.IsNullOrWhiteSpace(_bugTitleLineEdit.Text))
		{
			_bugTitleErrorLabel.Text = "Bug title is required.";
			_bugTitleErrorLabel.Visible = true;
			isValid = false;
		}
		else
		{
			_bugTitleErrorLabel.Visible = false;
		}

		// Validate Bug Description
		if (string.IsNullOrWhiteSpace(_bugDescriptionTextEdit.Text))
		{
			_bugDescriptionErrorLabel.Text = "Bug description is required.";
			_bugDescriptionErrorLabel.Visible = true;
			isValid = false;
		}
		else
		{
			_bugDescriptionErrorLabel.Visible = false;
		}

		// Validate Steps to Reproduce
		if (string.IsNullOrWhiteSpace(_stepsToReproduceTextEdit.Text))
		{
			_stepsToReproduceErrorLabel.Text = "Bug procedure steps are required.";
			_stepsToReproduceErrorLabel.Visible = true;
			isValid = false;
		}
		else
		{
			_stepsToReproduceErrorLabel.Visible = false;
		}

		return isValid;
	}

	private bool _isRequestSend = false;
	private void SubmitReportToServer(string title, string description, string stepsToReproduce)
	{
		if (_isRequestSend) { return; }
		_isRequestSend = true;
		_loadingComponent.Visible = true;
		string submitBugReportUrl = ApiClient.Instance.BuildUrl("BugReport");

		// prepare headers for the request
		var headers = new string[] { "Content-Type: application/json" };

		// prepare body for the request
		var data = new Godot.Collections.Dictionary
		{
			{"userId", UserDataManager.Instance.CurrentUser.Id ?? 0},
			{"title", title},
			{"description", description},
			{"stepsToReproduce", stepsToReproduce},
		};
		string body = Json.Stringify(data);
		GD.Print("Request send--temp");
		GD.Print(body);

		// submit request
		_httpRequest.Request(submitBugReportUrl, headers, HttpClient.Method.Post, body);
	}   

	private void OnHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		_isRequestSend = false;
		_loadingComponent.Visible = false;
		if (responseCode == 201)
		{
			_popupMessageComponentInstance.Visible = true;
			_popupMessageComponentScript.MessageLabel.Text = "Bug report submitted successfully. Thank you for your feedback!";
		}
		else if (responseCode >= 400)
		{
			_popupMessageComponentInstance.Visible = true;
			_popupMessageComponentScript.MessageLabel.Text = "Failed to submit bug report. Please try again later.";
		}
	}
}
