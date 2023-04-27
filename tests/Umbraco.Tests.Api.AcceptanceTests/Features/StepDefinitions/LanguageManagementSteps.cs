using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Umbraco.Cms.Api.Management.ViewModels.Language;

namespace Umbraco.Tests.Api.AcceptanceTests.Features.StepDefinitions;

[Binding]
public class LanguageManagementSteps
{
    private readonly HttpClient _httpClient;
    private readonly ScenarioContext _scenarioContext;

    public LanguageManagementSteps(HttpClient httpClient, ScenarioContext scenarioContext)
    {
        _httpClient = httpClient;
        _scenarioContext = scenarioContext;
    }

    [When(@"I create a new language with the following details")]
    public async Task WhenICreateANewLanguageWithTheFollowingDetails(Table table)
    {
        var language = table.CreateInstance<CreateLanguageRequestModel>();
        _scenarioContext.Add("createLanguageRequestModel", language);
        var response = await _httpClient.PostAsJsonAsync("language", language);
        _scenarioContext.Add("languageLocation", response.Headers.Location);
    }

    [Then(@"The language should be created successfully")]
    public async Task ThenTheLanguageShouldBeCreatedSuccessfully()
    {
        var languageLocation = _scenarioContext.Get<Uri?>("languageLocation");
        var languageResponse = await _httpClient.GetAsync(languageLocation);
        var createdLanguage = await languageResponse.Content.ReadFromJsonAsync<LanguageResponseModel>();
        var language = _scenarioContext.Get<CreateLanguageRequestModel>("createLanguageRequestModel");

        createdLanguage!.IsoCode.Should().BeEquivalentTo(language.IsoCode);
        createdLanguage.Name.Should().BeEquivalentTo(language.Name);
        createdLanguage.IsDefault.Should().Be(language.IsDefault);
        createdLanguage.IsMandatory.Should().Be(language.IsMandatory);
    }

    [AfterScenario("createLanguageItem")]
    public void DeleteLanguage()
    {
        var languageLocation = _scenarioContext.Get<Uri?>("languageLocation");
        _httpClient.DeleteAsync(languageLocation);
    }

    [Given(@"I have a language with the following details")]
    public async Task GivenIHaveALanguageWithTheFollowingDetails(Table table)
    {
        var language = table.CreateInstance<CreateLanguageRequestModel>();
        var response = await _httpClient.PostAsJsonAsync("language", language);
        _scenarioContext.Add("languageLocation", response.Headers.Location);
    }

    [When(@"that language gets deleted")]
    public async Task WhenThatLanguageGetsDeleted()
    {
        var languageLocation = _scenarioContext.Get<Uri?>("languageLocation");
        await _httpClient.DeleteAsync(languageLocation);
    }

    [Then(@"The language should be deleted successfully")]
    public async Task ThenTheLanguageShouldBeDeletedSuccessfully()
    {
        var languageLocation = _scenarioContext.Get<Uri?>("languageLocation");
        var response = await _httpClient.GetAsync(languageLocation);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
