namespace CarteiraInvest.Integracoes.Brapi;

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Timers;

public sealed class QuoteResponse
{
    [JsonPropertyName("results")] public List<QuoteResult> Results { get; set; } = new();
}

public sealed class QuoteResult
{
    [JsonPropertyName("symbol")] public string Symbol { get; set; } = "";
    [JsonPropertyName("longName")] public string LongName { get; set; } = "";
    [JsonPropertyName("regularMarketPrice")] public decimal Price { get; set; }
    [JsonPropertyName("data")] public QuoteData Data { get; set; } = new();
}

public sealed class QuoteData
{
    [JsonPropertyName("regularMarketPrice")] public decimal Price { get; set; }
    [JsonPropertyName("regularMarketChangePercent")] public decimal ChangePercent { get; set; }
    [JsonPropertyName("regularMarketVolume")] public long? Volume { get; set; }
}

public sealed class BrapiClient(HttpClient http)
{
    public async Task<List<QuoteResult>> GetQuotesAsync(string symbols, CancellationToken ct = default)
    {
        var resp = await http.GetAsync($"/api/v2/stocks/quote?symbols={Uri.EscapeDataString(symbols)}", ct);
        if (!resp.IsSuccessStatusCode)
            throw new HttpRequestException($"brapi {(int)resp.StatusCode}: {await resp.Content.ReadAsStringAsync(ct)}");

        var body = await resp.Content.ReadFromJsonAsync<QuoteResponse>(cancellationToken: ct);
        return body?.Results ?? new();
    }
    public async Task<QuoteResult> GetSymbolAsync(string symbol, CancellationToken ct = default)
    {
        var resp = await http.GetAsync($"/api/quote/{symbol}");
        if (!resp.IsSuccessStatusCode)
            throw new HttpRequestException($"brapi {(int)resp.StatusCode}: {await resp.Content.ReadAsStringAsync(ct)}");

        var body = await resp.Content.ReadFromJsonAsync<QuoteResponse>(cancellationToken: ct);
        return body?.Results.FirstOrDefault() ?? throw new InvalidDataException($"Não foi encontrada ação para o {symbol}");
    }
}
//* Configuração do HttpClient para o BrapiClient
//builder.Services.AddHttpClient<BrapiClient>(c =>
//{
//    c.BaseAddress = new Uri("https://brapi.dev/");
//c.DefaultRequestHeaders.Authorization =
//        new AuthenticationHeaderValue("Bearer", builder.Configuration["Brapi:Token"]);
//c.Timeout = TimeSpan.FromSeconds(15);
//});
