// AutomacaoIframeDemo/Contexts/SharedScenarioContext.cs
using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TechTalk.SpecFlow;
using AutomacaoIframeDemo.Services;

namespace AutomacaoIframeDemo.Contexts
{
    [Binding]
    public class SharedScenarioContext
    {
        public IWebDriver Driver { get; private set; }
        public ScenarioDataContext ScenarioData { get; private set; }
        public List<string> SharedModules { get; set; } = new List<string>();
        private IDigitacaoService _digitacaoService;

        [BeforeScenario]
        public void BeforeScenario()
        {
            var options = new ChromeOptions();

            // Headless ativado por variável de ambiente — mesmo padrão do projeto original.
            // Localmente: roda com UI. No GitHub Actions: HEADLESS=true ativa modo headless.
            if (Environment.GetEnvironmentVariable("HEADLESS") == "true")
            {
                options.AddArgument("--headless=new");
            }

            // Argumentos mantidos do projeto original
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-infobars");
            options.AddArgument("--remote-allow-origins=*");
            options.AddArgument("--disable-background-networking");
            options.AddArgument("--disable-client-side-phishing-detection");
            options.AddArgument("--disable-default-apps");
            options.AddArgument("--disable-sync");
            options.AddArgument("--disable-translate");

            Driver = new ChromeDriver(options);
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            _digitacaoService = new DigitacaoService();
            ScenarioData = new ScenarioDataContext();
            SharedModules.Clear();
        }

        [AfterScenario]
        public void AfterScenario()
        {
            try { Driver?.Quit(); } catch { }
        }

        public T GetService<T>() where T : class
        {
            if (typeof(T) == typeof(IDigitacaoService))
                return _digitacaoService as T;

            return null;
        }
    }
}
