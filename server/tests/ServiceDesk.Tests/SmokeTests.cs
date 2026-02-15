namespace ServiceDesk.Tests;

public class SmokeTests
{
    [Fact]
    public void ApplicationAssembly_Loads()
    {
        var assemblyName = typeof(ServiceDesk.Application.DependencyInjection).Assembly.GetName().Name;
        Assert.Equal("ServiceDesk.Application", assemblyName);
    }
}
