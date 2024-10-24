namespace MauiApp1;
using Services;
using SkiaSharp;
using SkiaSharp.Views.Maui;

public partial class MainPage : ContentPage
{
    private List<int> _originalArray = new List<int>();
    private List<int> _sortedArray = new List<int>();
    private Dictionary<string, (List<int> sortedArray, int iterations)> _sortingResults = new Dictionary<string, (List<int>, int)>();


    public MainPage()
	{
		InitializeComponent();
        SQLitePCL.Batteries_V2.Init();

    }

    // Event for array save button
    private async void OnSaveArrayClicked(object sender, EventArgs e)
    {
        if (ParseInputArray())
        {
            var dbService = new DatabaseService();
            await dbService.SaveArrayAsync(_originalArray);

            await DisplayAlert("Saved", "The array was saved successfully.", "OK");
        }
        else
        {
            await DisplayAlert("Error", "Enter array before saving", "OK");
        }
    }

    // Event for array load button
    private async void OnLoadArrayClicked(object sender, EventArgs e)
    {
        var dbService = new DatabaseService();

        List<int> array = await dbService.GetAllNumbersAsync();
        InputArrayEntry.Text = string.Join(" ", array);


        await DisplayAlert("Uploaded", "The array was loaded successfully.", "OK");
    }

    
    // Event for array sort button
    private void OnSortArrayClicked(object sender, EventArgs e)
    {
        if (!ParseInputArray())
        {
            DisplayAlert("Error", "Invalid array input", "OK");
            return;
        }

        // Sorting by all methods
        PerformSorting();

        // Update histograms and best method
        UpdateHistograms();
        DisplayBestMethod();
    }

    private void OnOriginalArrayPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        DrawHistogram(e.Surface.Canvas, e.Info.Width, e.Info.Height, _originalArray);
    }

    private void OnSortedArrayPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        DrawHistogram(e.Surface.Canvas, e.Info.Width, e.Info.Height, _sortedArray);
    }

    private void DrawHistogram(SKCanvas canvas, int width, int height, List<int> array)
    {
        canvas.Clear(SKColors.White);

        if (array == null || array.Count == 0) return;

        // Drawing settings
        var paint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Blue,
            IsAntialias = true
        };

        // Finding the maximum value for normalizing column heights
        int maxValue = array.Max();

        // Calculating the width of one column
        float barWidth = width / (float)array.Count;

        // Drawing 
        for (int i = 0; i < array.Count; i++)
        {
            float barHeight = (array[i] / (float)maxValue) * height;
            canvas.DrawRect(i * barWidth, height - barHeight, barWidth - 5, barHeight, paint);
        }
    }
    // Method for updating histograms
    private void UpdateHistograms()
    {

        OriginalArrayCanvas.InvalidateSurface();
        SortedArrayCanvas.InvalidateSurface();
    }

    // Method for choosing the best sorting method
    private void DisplayBestMethod()
    {
        var bestMethod = _sortingResults.OrderBy(r => r.Value.iterations).First();
        BestMethodLabel.Text = $"Best method:{bestMethod.Key} ({bestMethod.Value.iterations} iterations)";
    }

    // Method to handle change of selected method in DropDownList
    private void OnMethodPickerChanged(object sender, EventArgs e)
    {
        var selectedMethod = MethodPicker.SelectedItem.ToString();
        if (_sortingResults.ContainsKey(selectedMethod))
        {
            var result = _sortingResults[selectedMethod];
            MethodInfoLabel.Text = $"{selectedMethod}: {result.iterations} iterations";
        }
    }

    
    // Method for performing sorting with all methods
    private void PerformSorting()
    {
        _sortingResults.Clear();

        // Bubble Sort
        _sortingResults["Bubble"] = SortAlgorithms.BubbleSort(_originalArray.ToArray());

        // Merge Sort
        _sortingResults["Merge"] = SortAlgorithms.MergeSort(_originalArray.ToArray());

        // Heap Sort
        _sortingResults["Heap"] = SortAlgorithms.HeapSort(_originalArray.ToArray());

        // Quick Sort
        _sortingResults["Quick"] = SortAlgorithms.QuickSort(_originalArray.ToArray());

        // Radix Sort
        _sortingResults["Radix"] = SortAlgorithms.RadixSort(_originalArray.ToArray());

        _sortedArray = _sortingResults["Bubble"].sortedArray;

    }

    // Method for parsing array input
    private bool ParseInputArray()
    {
        try
        {
            _originalArray = InputArrayEntry.Text
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList();
            if (_originalArray.Count == 0)
                throw new Exception("Number of elements in array 0");
            return true;
        }
        catch(Exception ex)
        {
            return false;
        }
    }

    // Method for displaying "about the developer"
    private async void OnInstructionsClicked(object sender, EventArgs e)
    {
        var developerInfoView = new DeveloperInfoView();
        var contentPage = new ContentPage
        {
            Content = developerInfoView.Content
        };

        await Shell.Current.Navigation.PushModalAsync(contentPage);
    }
}

