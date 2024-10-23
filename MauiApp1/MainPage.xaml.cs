namespace MauiApp1;
using Services;

public partial class MainPage : ContentPage
{
    private List<int> _originalArray = new List<int>();
    private Dictionary<string, (List<int> sortedArray, int iterations)> _sortingResults = new Dictionary<string, (List<int>, int)>();


    public MainPage()
	{
		InitializeComponent();
        SQLitePCL.Batteries_V2.Init();

    }
    #region in development
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
    #endregion
    
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

    #region in development
    // Method for updating histograms
    private void UpdateHistograms()
    {
        // Здесь необходимо обновить графические компоненты гистограмм с данными
        // Для простоты выводим текст
        DisplayAlert("Гистограмма до", string.Join(", ", _originalArray), "OK");
        DisplayAlert("Гистограмма после", string.Join(", ", _sortingResults["Быстрая сортировка"].sortedArray), "OK");
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
    #endregion
    
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
        await DisplayAlert("About the developer", "Something", "OK");
    }


}

