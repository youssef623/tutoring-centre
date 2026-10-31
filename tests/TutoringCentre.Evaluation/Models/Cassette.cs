using System.Text.Json;
using System.Text.RegularExpressions;
using TutoringCentre.Application.Assistant.Models;
using TutoringCentre.Evaluation.Cases;
namespace TutoringCentre.Evaluation.Models;

public sealed record CassetteEntry(string RequestHash, ModelRequest Request, ModelResponse Response);
public sealed record Cassette(string CaseId, string PromptVersion, string Model, CassetteEntry[] Entries)
{
    private static readonly JsonSerializerOptions ScanJson = new(CaseLoader.Json)
    {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    public static Cassette Read(string path)
    {
        if (!File.Exists(path)) throw new FileNotFoundException("eval.cassette_missing", path);
        var cassette = JsonSerializer.Deserialize<Cassette>(File.ReadAllText(path), CaseLoader.Json) ?? throw new InvalidDataException("eval.cassette_invalid");
        if (cassette.Entries.Length == 0 || cassette.Entries.Any(e => e.RequestHash != CassetteKey.Create(cassette.PromptVersion, e.Request))) throw new InvalidDataException("eval.cassette_invalid");
        return cassette;
    }
    public async Task WriteAsync(string path, IEnumerable<string> secrets)
    {
        if (Entries.Length == 0) throw new InvalidDataException("eval.recording_empty");
        var text = JsonSerializer.Serialize(this, CaseLoader.Json);
        var scanText = JsonSerializer.Serialize(this, ScanJson);
        if (secrets.Where(s => !string.IsNullOrEmpty(s)).Any(s => scanText.Contains(s, StringComparison.Ordinal)) || Regex.IsMatch(scanText, @"(?i)(sk-[a-z0-9_-]{12,}|Bearer [a-z0-9_.-]{12,}|AccountKey=|BEGIN .*PRIVATE KEY)", RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1))) throw new InvalidDataException("eval.secret_detected");
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        await File.WriteAllTextAsync(path, text);
    }
}
