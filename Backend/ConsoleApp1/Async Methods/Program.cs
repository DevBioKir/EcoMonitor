
WaitAndReturnAsync();
async Task<string> WaitAndReturnAsync()
{
    await Task.Delay(3000);
    return "Done";
}


// Асинхронно загрузить
async Task<List<string>> LoadSequentialAsync(List<string> urls)
{
    List<string> results = new List<string>();

    foreach (var url in urls)
    {
       string content = await LoadAsync(url);
        results.Add(content);
    }
    return results;
}

// Параллельно загрузить
async Task<List<string>> LoadParallelAsync(List<string> urls)
{
    using var client = new HttpClient();

    var tasks = urls.Select(url => client.GetStringAsync(url));

    var result = await Task.WhenAll(tasks);

    return result.ToList();
}

async Task<string> LoadAsync(string url)
{
    using var client = new HttpClient();
    return await client.GetStringAsync(url);
}
