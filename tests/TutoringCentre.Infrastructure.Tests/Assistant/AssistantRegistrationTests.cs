using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using TutoringCentre.Application.Assistant;
using TutoringCentre.Application.Assistant.Models;
using TutoringCentre.Infrastructure.Assistant;

namespace TutoringCentre.Infrastructure.Tests.Assistant;

public sealed class AssistantRegistrationTests
{
    [Fact]
    public void Production_RejectsScriptedEvenWithExistingFile()
    {
        using var script = new ScriptFile();
        using var services = Services("Production", new() { ["Assistant:Provider"] = "scripted", ["Assistant:ScriptPath"] = script.Path });
        Assert.Throws<OptionsValidationException>(() => services.GetRequiredService<IOptions<ModelProviderOptions>>().Value);
    }

    [Fact]
    public void Live_RejectsMissingKey()
    {
        using var services = Services("Development", new() { ["Assistant:Provider"] = "live", ["Assistant:Model"] = "model" });
        Assert.Throws<OptionsValidationException>(() => services.GetRequiredService<IOptions<ModelProviderOptions>>().Value);
    }

    [Theory]
    [InlineData("00:00:00")]
    [InlineData("-00:00:01")]
    public void InvalidTimeout_IsRejected(string timeout)
    {
        using var services = Services("Testing", new() { ["Assistant:CallTimeout"] = timeout });
        Assert.Throws<OptionsValidationException>(() => services.GetRequiredService<IOptions<AssistantOptions>>().Value);
    }

    [Fact]
    public async Task Scripted_ReadsFileHonoursCancellationAndFailsOnExhaustion()
    {
        using var script = new ScriptFile();
        using var services = Services("Testing", new() { ["Assistant:Provider"] = "scripted", ["Assistant:ScriptPath"] = script.Path });
        var model = services.GetRequiredService<ILanguageModel>();
        var request = new ModelRequest("System", [], [], 10);
        using var cancelled = new CancellationTokenSource();
        await cancelled.CancelAsync();
        Assert.True((await model.GenerateAsync(request, cancelled.Token)).IsFailure);
        var result = await model.GenerateAsync(request, CancellationToken.None);
        Assert.Equal("Canned reply", result.Value.Text);
        Assert.True((await model.GenerateAsync(request, CancellationToken.None)).IsFailure);
    }

    private static ServiceProvider Services(string environment, Dictionary<string, string?> values) => new ServiceCollection()
        .AddSingleton<IHostEnvironment>(new EnvironmentStub(environment)).AddLogging()
        .AddAssistant(new ConfigurationBuilder().AddInMemoryCollection(values).Build()).BuildServiceProvider();

    private sealed class EnvironmentStub(string name) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = name;
        public string ApplicationName { get; set; } = "Tests";
        public string ContentRootPath { get; set; } = ".";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }

    private sealed class ScriptFile : IDisposable
    {
        public string Path { get; } = System.IO.Path.GetTempFileName();
        public ScriptFile() => File.WriteAllText(Path, """[{"text":"Canned reply","toolCalls":[],"usage":{"inputTokens":1,"outputTokens":2},"stopReason":"completed"}]""");
        public void Dispose() => File.Delete(Path);
    }
}
