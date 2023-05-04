using Godot;
using System;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Text.Json;

public partial class RequestHud : CanvasLayer
{
	[Signal]
	public delegate void BackToMainMenuEventHandler();
	
	private readonly System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void OnBackButtonPressed()
	{
		GetNode<PromptHud>("PromptHUD").ClearContent();
		EmitSignal(SignalName.BackToMainMenu);
	}
	
	private void OnSendRequestButtonPressed()
	{
		GetNode<Button>("SendRequestButton").Disabled = true;
		GetNode<Button>("SaveButton").Disabled = true;
		GetNode<Button>("BackButton").Disabled = true;
		
		new Thread(new ThreadStart(async () => {
			Dictionary<string, string> values = new Dictionary<string, string>
			{
				{ "prompt", GetNode<LineEdit>("PromptHUD/PromptInput").Text },
				{ "steps", "25" }
			};
			
			var body = JsonContent.Create(values, values.GetType());
			var response = await client.PostAsync("http://localhost:9999/sd", body);
  			var responseString = await response.Content.ReadAsStringAsync();
			
			Dictionary<string, string[]> imagesJson =
				JsonSerializer.Deserialize<Dictionary<string, string[]>>(responseString);
			
			GD.Print(imagesJson["images"][0]);
			
			byte[] imageBytes = Convert.FromBase64String(imagesJson["images"][0]);
			Image img = new Image();
			img.LoadPngFromBuffer(imageBytes);
			GetNode<TextureRect>("ImageDisplay").Texture = ImageTexture.CreateFromImage(img);
			
			GetNode<Button>("SendRequestButton").Disabled = false;
			GetNode<Button>("SaveButton").Disabled = false;
			GetNode<Button>("BackButton").Disabled = false;
		})).Start();
	}
}

