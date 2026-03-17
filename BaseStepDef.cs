// AutomacaoIframeDemo/Steps/BaseStepDef.cs
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using AutomacaoIframeDemo.Contexts;
using AutomacaoIframeDemo.Services;

namespace AutomacaoIframeDemo.Steps
{
    public abstract class BaseStepDef
    {
        protected readonly IWebDriver Driver;
        protected readonly ScenarioDataContext Context;
        protected readonly IDigitacaoService DigitacaoService;

        protected BaseStepDef(SharedScenarioContext sharedContext)
        {
            Driver          = sharedContext.Driver;
            Context         = sharedContext.ScenarioData;
            DigitacaoService = sharedContext.GetService<IDigitacaoService>();
        }

        protected IWebElement WaitForElement(By locator, int seconds = 10)
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(seconds));
            return wait.Until(ExpectedConditions.ElementIsVisible(locator));
        }

        /// <summary>
        /// Alterna o contexto do WebDriver para o iframe do módulo de notas.
        /// No mock: id="notas-frame" | No projeto original: #note-typing-iframe
        /// Este é o método central do projeto — o que torna Selenium necessário
        /// onde Cypress não consegue operar.
        /// </summary>
        protected void SwitchToNotasFrame()
        {
            // Use a espera que troca para o frame de forma segura (evita stale element)
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.FrameToBeAvailableAndSwitchToIt(By.CssSelector("#notas-frame")));

            try
            {
                var readyWait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5));
                readyWait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").ToString() == "complete");
            }
            catch
            {
                // se não completar em 5s, deixamos continuar e as esperas posteriores cuidarão disso
            }
        }

        protected void SwitchToDefaultContent()
        {
            Driver.SwitchTo().DefaultContent();
        }

        /// <summary>
        /// Executa uma ação dentro do iframe e garante retorno ao contexto principal,
        /// mesmo em caso de exceção — padrão try/finally mantido do projeto original.
        /// </summary>
        protected void ExecuteInNotasFrame(Action action)
        {
            SwitchToNotasFrame();
            try
            {
                action();
            }
            finally
            {
                SwitchToDefaultContent();
            }
        }
    }
}
