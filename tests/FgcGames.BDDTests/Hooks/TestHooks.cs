using FgcGames.BDDTests.Support;
using Reqnroll;

namespace FgcGames.BDDTests.Hooks;

[Binding]
public class TestHooks(ApiContext apiContext)
{
    [BeforeTestRun]
    public static async Task BeforeTestRun()
        => await TestEnvironment.InitializeAsync();

    [AfterTestRun]
    public static async Task AfterTestRun()
        => await TestEnvironment.DisposeAsync();

    [BeforeScenario]
    public async Task BeforeScenario()
    {
        await TestEnvironment.ResetAsync();
        apiContext.Client = TestEnvironment.CreateClient();
        apiContext.LastResponse = null;
        apiContext.Token = null;
        apiContext.LastCreatedId = Guid.Empty;
    }

    [AfterScenario]
    public void AfterScenario()
        => apiContext.Client?.Dispose();
}
