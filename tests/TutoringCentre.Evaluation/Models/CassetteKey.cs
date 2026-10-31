using System.Security.Cryptography;
using System.Text.Json;
using TutoringCentre.Application.Assistant.Models;
namespace TutoringCentre.Evaluation.Models;

public static class CassetteKey
{
    public static string Create(string promptVersion, ModelRequest request) => Convert.ToHexString(SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(new { promptVersion, request.System, request.Messages, request.Tools, request.MaxOutputTokens })));
}
