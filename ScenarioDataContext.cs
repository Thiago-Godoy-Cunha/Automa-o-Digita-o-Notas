// AutomacaoIframeDemo/Contexts/ScenarioDataContext.cs
using System.Collections.Generic;
using TechTalk.SpecFlow;

namespace AutomacaoIframeDemo.Contexts
{
    [Binding]
    public class ScenarioDataContext
    {
        public Dictionary<string, object> Data { get; } = new Dictionary<string, object>();

        [BeforeScenario]
        public void ClearData()
        {
            Data.Clear();
        }
    }
}
