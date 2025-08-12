

async Task<IEnumerable<string>> GetHtmlPagesAsync(List<string> urls)
{
    using var client = new HttpClient();

    var tasks = urls.Select(url => client.GetStringAsync(url));

    return (await Task.WhenAll(tasks)).ToList();
}
