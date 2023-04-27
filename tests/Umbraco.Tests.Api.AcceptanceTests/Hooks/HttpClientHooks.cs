using BoDi;
using TechTalk.SpecFlow;

namespace Umbraco.Tests.Api.AcceptanceTests.Hooks;

[Binding]
public class HttpClientHooks
{
    private IObjectContainer _objectContainer;

    public HttpClientHooks(IObjectContainer objectContainer)
    {
        _objectContainer = objectContainer;
    }

    [BeforeScenario]
    public void AddHttpClient() =>
        _objectContainer.RegisterInstanceAs(new HttpClient
        {
            BaseAddress = new Uri("https://localhost:44331/umbraco/management/api/v1.0/")
        });
}
