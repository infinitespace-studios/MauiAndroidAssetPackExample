namespace InstallTimeExample;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private async void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		var foo = await ReadTextFile ("Foo.txt");

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time. {foo}";
		else
			CounterBtn.Text = $"Clicked {count} times. {foo}";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}

	async Task<string> ReadTextFile(string filePath)
	{
		using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(filePath);
		using StreamReader reader = new StreamReader(fileStream);

		return await reader.ReadToEndAsync();
	}
}

