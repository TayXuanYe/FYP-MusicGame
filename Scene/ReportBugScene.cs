using Godot;
using System;

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

    public override void _Ready()
    {
        _backToHomeButton.Pressed += OnBackToHomeButtonPressed;
        _submitButton.Pressed += OnSubmitButtonPressed;
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
            // Handle bug report submission future enhancement: send to server
            GD.Print("Bug report submitted successfully!");
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

}
