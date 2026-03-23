// AutomacaoIframeDemo/Hooks/Hooks.cs
using TechTalk.SpecFlow;
using AutomacaoIframeDemo.Helpers;

namespace AutomacaoIframeDemo.Hooks
{
    [Binding]
    public class LogHooks
    {
        [BeforeScenario]
        public static void BeforeScenario(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            LogHelper.Initialize(featureContext, scenarioContext);
        }

        [AfterScenario]
        public static void AfterScenario()
        {
            LogHelper.FlushAndClose();
        }
    }
}
