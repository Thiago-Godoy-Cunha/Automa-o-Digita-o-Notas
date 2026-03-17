// AutomacaoIframeDemo/Steps/LoginStepDef.cs
using System;
using NUnit.Framework;
using OpenQA.Selenium;
using TechTalk.SpecFlow;
using AutomacaoIframeDemo.Contexts;
using AutomacaoIframeDemo.Helpers;

namespace AutomacaoIframeDemo.Steps
{
    [Binding]
    public class LoginStepDef : BaseStepDef
    {
        public LoginStepDef(SharedScenarioContext sharedContext)
            : base(sharedContext) { }

        /// <summary>
        /// No projeto original: navegava para a URL real da empresa e fazia login.
        /// No mock: abre diretamente o portal de demonstração (sem autenticação).
        /// A URL é lida de variável de ambiente — configurável na pipeline do GitHub Actions.
        /// </summary>
        [Given(@"que estou logado no portal de demonstração")]
        public void GivenQueEstouLogadoNoPortal()
        {
            var baseUrl = Environment.GetEnvironmentVariable("BASE_URL")
                          ?? "http://127.0.0.1:5500/index.html";

            LogHelper.WriteLine($"[Given] Abrindo portal de demonstração: {baseUrl}");
            Driver.Navigate().GoToUrl(baseUrl);
            LogHelper.WriteLine($"  URL atual: {Driver.Url}");

            // Aguarda o iframe estar presente antes de prosseguir
            WaitForElement(By.Id("notas-frame"));
            LogHelper.WriteLine("  Portal carregado — iframe de notas localizado");
        }
    }
}
