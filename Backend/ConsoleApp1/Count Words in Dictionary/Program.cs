

Dictionary<string, int> CountWords(string text)
{
    if (string.IsNullOrWhiteSpace(text))
        return new Dictionary<string, int>();

    return text
        .ToLower()
        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
        .GroupBy(word => word)
        .ToDictionary(g => g.Key, g => g.Count());
}
