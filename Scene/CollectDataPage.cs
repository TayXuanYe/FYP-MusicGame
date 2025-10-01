using Godot;
using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;

public partial class CollectDataPage : Control
{
	[Export] private Button _submitButton;
	[Export] private LineEdit _input;
	[Export] private Label _errorLabel;

	public override void _Ready()
	{
		_submitButton.Pressed += OnSubmitButtonPressed;
	}

	private void OnSubmitButtonPressed()
	{
		// Validate input before generating report
		string validationError = ValidateInput();
		if (string.IsNullOrEmpty(validationError))
		{
			// Clear any previous error message
			_errorLabel.Text = "";
			_errorLabel.Visible = false;
			
			// Generate report
			GenerateDataFolders();
			
			// Compress the output folder
			// ZipOutputFolder();

			SceneManager.Instance.ChangeToMainMenuScene();
		}
		else
		{
			// Display error message
			_errorLabel.Text = validationError;
			_errorLabel.Visible = true;
		}
	}

	private void GenerateDataFolders()
	{
		// Create Output directory if it doesn't exist
		string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "Output");
		if (!Directory.Exists(outputDir))
		{
			Directory.CreateDirectory(outputDir);
		}

		// Get the playlist chart IDs
		var playlistChartsId = GameProgressManger.Instance.PlaylistChartsId;
		
		// Create a folder for each chart
		for (int i = 0; i < playlistChartsId.Count; i++)
		{
			int chartId = playlistChartsId[i];
			
			// Create chart folder
			string chartFolder = Path.Combine(outputDir, $"Chart_{chartId}_{Time.GetUnixTimeFromSystem()}");
			if (!Directory.Exists(chartFolder))
			{
				Directory.CreateDirectory(chartFolder);
			}

			// Create RawUserInputData.txt
			string inputFilePath = Path.Combine(chartFolder, "RawUserInputData.txt");
			WriteInputData(inputFilePath, chartId, i);

			// Create RawUserGazeData.txt
			string gazeFilePath = Path.Combine(chartFolder, "RawUserGazeData.txt");
			WriteGazeData(gazeFilePath, i);

			// Create userData.txt
			string userDataFilePath = Path.Combine(chartFolder, "userData.txt");
			WriteUserData(userDataFilePath);

			// Create chart data folder and copy chart files
			string chartDataFolder = Path.Combine(chartFolder, "ChartData");
			CopyChartData(chartDataFolder, chartId);
		}
	}

	private void WriteInputData(string filePath, int chartId, int playIndex)
	{
		using (var writer = new StreamWriter(filePath))
		{
			// Write header
			writer.WriteLine($"Chart ID: {chartId}");
			writer.WriteLine($"Play Index: {playIndex}");
			writer.WriteLine(""); // Empty line for readability

			// Write input data for this chart
			if (GameProgressManger.Instance.RawUserInputData.ContainsKey(playIndex))
			{
				foreach (var input in GameProgressManger.Instance.RawUserInputData[playIndex])
				{
					if (input.NoteType == "Tap")
					{
						writer.WriteLine($"NoteType: {input.NoteType}, LaneIndex: {input.LaneIndex}, HitResult: {input.HitResult}, HitTime: {input.HitTime}, TimeDifference: {input.TimeDifference}, TargetHitTime: {input.TargetHitTime}, SystemTime: {input.SystemTime}");
					}
					else if (input.NoteType == "Hold")
					{
						writer.WriteLine($"NoteType: {input.NoteType}, LaneIndex: {input.LaneIndex}, HitResult: {input.HitResult}, HitTime: {input.HitTime}, TargetHitTime: {input.TargetHitTime}, SystemTime: {input.SystemTime}, DurationTime: {input.DurationTime}, HoldTotalTime: {input.HoldTotalTime}, HoldTimeRatio: {input.HoldTimeRatio}");
					}
				}
			}
		}
	}

	private void WriteGazeData(string filePath, int playIndex)
	{
		using (var writer = new StreamWriter(filePath))
		{
			// Write header
			writer.WriteLine($"Play Index: {playIndex}");
			writer.WriteLine(""); // Empty line for readability

			// Write gaze data for this play index
			if (GameProgressManger.Instance.RawUserGazeData.ContainsKey(playIndex))
			{
				foreach (var gaze in GameProgressManger.Instance.RawUserGazeData[playIndex])
				{
					writer.WriteLine($"X: {gaze.X}, Y: {gaze.Y}, Confidence: {gaze.Confidence}, Timestamp: {gaze.Timestamp}");
				}
			}
		}
	}

	private void CopyChartData(string chartDataFolder, int chartId)
	{
		// Create chart data folder
		if (!Directory.Exists(chartDataFolder))
		{
			Directory.CreateDirectory(chartDataFolder);
		}

		// Get the source chart file path
		if (ChartManager.Instance.ChartsIdIndex.ContainsKey(chartId))
		{
			string sourceChartPath = ChartManager.Instance.ChartsIdIndex[chartId];
			string sourceDirectory = Path.GetDirectoryName(sourceChartPath);
			
			// Copy the data.txt file
			string destDataPath = Path.Combine(chartDataFolder, "data.txt");
			if (File.Exists(sourceChartPath))
			{
				File.Copy(sourceChartPath, destDataPath, true);
			}

			// Copy music files if they exist
			string[] supportedFormats = { ".ogg", ".wav", ".mp3" };
			foreach (var format in supportedFormats)
			{
				string sourceMusicPath = Path.Combine(sourceDirectory, $"music{format}");
				if (File.Exists(sourceMusicPath))
				{
					string destMusicPath = Path.Combine(chartDataFolder, $"music{format}");
					File.Copy(sourceMusicPath, destMusicPath, true);
					break; // Only copy one music file
				}
			}
		}
	}

	private void WriteUserData(string filePath)
	{
		using (var writer = new StreamWriter(filePath))
		{
			// Write SuggestedDifficulty from UserDataManager
			string suggestedDifficulty = UserDataManager.Instance.CurrentUser.SuggestedDifficulty ?? "NotSet";
			writer.WriteLine($"SuggestedDifficulty: {suggestedDifficulty}");

			// Get and validate input value
			string inputText = _input.Text ?? "";
			writer.WriteLine($"InputValue: {inputText}");

			// Validate input value
			if (double.TryParse(inputText, out double inputValue))
			{
				if (inputValue > 0)
				{
					writer.WriteLine("InputValidation: Valid");
				}
				else
				{
					writer.WriteLine("InputValidation: Invalid - Value must be positive");
				}
			}
			else
			{
				writer.WriteLine("InputValidation: Invalid - Not a valid number");
			}
		}
	}

	private string ValidateInput()
	{
		// Check if input is empty
		if (string.IsNullOrWhiteSpace(_input.Text))
		{
			return "Input value is required.";
		}

		// Check if input is a valid number
		if (!double.TryParse(_input.Text, out double inputValue))
		{
			return "Input value must be a valid number.";
		}

		// Check if input is positive
		if (inputValue <= 0)
		{
			return "Input value must be a positive number.";
		}

		// Input is valid
		return null;
	}

	private void ZipOutputFolder()
	{
		try
		{
			string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "Output");
			if (Directory.Exists(outputDir))
			{
				// Create zip file name with timestamp
				string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
				string zipFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"Output_{timestamp}.zip");
				
				// Create zip file
				ZipFile.CreateFromDirectory(outputDir, zipFilePath);
				
				// Optionally, you can show a success message or log it
				GD.Print($"Output folder compressed successfully to: {zipFilePath}");
			}
			else
			{
				GD.PrintErr("Output directory does not exist.");
			}
		}
		catch (Exception ex)
		{
			GD.PrintErr($"Failed to compress output folder: {ex.Message}");
			// Display error message to user
			_errorLabel.Text = "Failed to compress output folder.";
			_errorLabel.Visible = true;
		}
	}
}
