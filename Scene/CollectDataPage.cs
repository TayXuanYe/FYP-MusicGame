using Godot;
using System;

public partial class CollectDataPage : Control
{
    [Export] private Button _submitButton;

    public override void _Ready()
    {
        _submitButton.Pressed += OnSubmitButtonPressed;
    }

    private void OnSubmitButtonPressed()
    {
        // Generate report
        
    }

}
