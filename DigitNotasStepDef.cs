// AutomacaoIframeDemo/Steps/DigitNotasStepDef.cs
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using TechTalk.SpecFlow;
using AutomacaoIframeDemo.Contexts;
using AutomacaoIframeDemo.Domain;
using AutomacaoIframeDemo.Helpers;
using AutomacaoIframeDemo.Services;

namespace AutomacaoIframeDemo.Steps
{
    [Binding]
    public class DigitNotasStepDef : BaseStepDef
    {
        private readonly IDigitacaoService _digitacaoService;
        private readonly SharedScenarioContext _sharedContext;
        private readonly Random _random = new Random();
        public DigitNotasStepDef(SharedScenarioContext sharedContext)
            : base(sharedContext)
        {
            _sharedContext   = sharedContext;
            _digitacaoService = sharedContext.GetService<IDigitacaoService>();
        }

        [Given(@"que acesso a página de Notas da turma ""(.*)""")]
        public void GivenQueAcessoAPaginaDeNotas(string inputTurma)
        {
            LogHelper.WriteLine($"Acessando notas da turma: '{inputTurma}'");
            var turmas = InputProcessor.ParseStringList(inputTurma);
            _digitacaoService.AdicionarTurmas(turmas);

            var primeiraTurma = _digitacaoService.ObterProximaTurmaParaProcessar();
            AcessarTurma(primeiraTurma);
        }

        [When(@"seleciono módulo ""(.*)""")]
        public void WhenSelecionoModulo(string inputModulo)
        {
            LogHelper.WriteLine($"Selecionando módulo: '{inputModulo}'");
            var turmaAtual = _digitacaoService.ObterTurmaAtual();
            if (turmaAtual == null) return;

            var modulos = InputProcessor.ParseStringList(inputModulo);
            _sharedContext.SharedModules = new List<string>(modulos);
            _digitacaoService.AdicionarModulosParaTurmaAtual(modulos);

            var moduloAtual = turmaAtual.ObterModuloAtual();
            AcessarModulo(moduloAtual);
        }

        [When(@"digito notas aleatoriamente num range de ""(.*)"" a ""(.*)"" na ""(.*)""° parcial")]
        public void WhenDigitoNotasAleatoriamente(float notaMinima, float notaMaxima, string inputParcial)
        {
            var turmaAtual = _digitacaoService.ObterTurmaAtual();
            var moduloAtual = turmaAtual?.ObterModuloAtual();

            if (moduloAtual == null)
            {
                LogHelper.WriteLine("Nenhum módulo atual encontrado para digitação");
                return;
            }

            LogHelper.WriteLine($"Digitando notas — Turma: {turmaAtual?.Codigo} | Módulo: {moduloAtual?.Nome}");
            ProcessarModuloCompleto(moduloAtual, notaMinima, notaMaxima, inputParcial);

            LogHelper.WriteLine($"Módulo '{moduloAtual.Nome}' concluído");
            moduloAtual.Processado = true;

            ProcessarProximoModulo(turmaAtual, notaMinima, notaMaxima, inputParcial);
        }

        [Then(@"devo ver o módulo selecionado disponível para alteração")]
        public void ThenDevoVerOModuloSelecionadoDisponivel()
        {
            var turmaAtual = _digitacaoService.ObterTurmaAtual();
            var moduloAtual = turmaAtual?.ObterModuloAtual();
            if (moduloAtual == null) return;

            ExecuteInNotasFrame(() =>
            {
                var elementosParciais = LocalizarParciaisDisponiveis();

                moduloAtual.Parciais = elementosParciais.Select((elem, index) => new Parcial
                {
                    Nome     = $"Parcial {index + 1}",
                    Numero   = index + 1,
                    Elemento = elem
                }).ToList();

                moduloAtual.DisponivelParaAlteracao = moduloAtual.Parciais.Any();
                LogHelper.WriteLine($"Parciais encontradas: {moduloAtual.Parciais.Count}");

                Assert.That(moduloAtual.DisponivelParaAlteracao, Is.True,
                    "Nenhum campo editável encontrado no iframe");
            });
        }
        private void ProcessarModuloCompleto(Modulo modulo, float notaMinima, float notaMaxima, string inputParcial)
        {
            var parciaisSolicitadas = InputProcessor.ParseIntegerList(inputParcial);
            LogHelper.WriteLine($"Parciais solicitadas: {string.Join(", ", parciaisSolicitadas)}");

            foreach (var numParcial in parciaisSolicitadas)
            {
                var parcial = modulo.Parciais.FirstOrDefault(p => p.Numero == numParcial);
                if (parcial == null)
                {
                    LogHelper.WriteLine($"Parcial {numParcial} não encontrada");
                    continue;
                }

                LogHelper.WriteLine($"Processando parcial {parcial.Numero}: {parcial.Nome}");

                if (parcial.IsArredondamento)
                    ProcessarParcial(parcial, 0, 0.5f);
                else if (!parcial.IsFinal)
                    ProcessarParcial(parcial, notaMinima, notaMaxima);

                parcial.Processada = true;
            }
            SalvarNotas();
        }

        private void ProcessarProximoModulo(Turma turmaAtual, float notaMinima, float notaMaxima, string inputParcial)
        {
            if (turmaAtual == null) return;

            var proximoModulo = turmaAtual.Modulos.FirstOrDefault(m => !m.Processado);
            if (proximoModulo == null)
            {
                LogHelper.WriteLine($"Turma '{turmaAtual.Codigo}' finalizada");
                return;
            }

            LogHelper.WriteLine($"Próximo módulo: {proximoModulo.Nome} na turma {turmaAtual.Codigo}");
            AcessarModulo(proximoModulo);

            ExecuteInNotasFrame(() =>
            {
                try
                {
                    var elementosParciais = LocalizarParciaisDisponiveis();
                    proximoModulo.Parciais = elementosParciais.Select((elem, index) => new Parcial
                    {
                        Nome     = $"Parcial {index + 1}",
                        Numero   = index + 1,
                        Elemento = elem
                    }).ToList();

                    if (proximoModulo.Parciais.Count == 0)
                        LogHelper.WriteLine($"Nenhuma parcial encontrada para módulo {proximoModulo.Nome}");
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLine($"Erro ao localizar parciais: {ex.Message}");
                }
            });

            ProcessarModuloCompleto(proximoModulo, notaMinima, notaMaxima, inputParcial);
            proximoModulo.Processado = true;
            LogHelper.WriteLine($"Módulo '{proximoModulo.Nome}' finalizado");
            ProcessarProximoModulo(turmaAtual, notaMinima, notaMaxima, inputParcial);
        }
        private void AcessarTurma(Turma turma)
        {
            var codigo = turma?.Codigo ?? string.Empty;
            LogHelper.WriteLine($"Acessando turma: {codigo}");

            ExecuteInNotasFrame(() =>
            {
                try
                {
                    var select = new OpenQA.Selenium.Support.UI.SelectElement(
                        Driver.FindElement(By.Id("turma")));
                    select.SelectByValue(codigo);
                    LogHelper.WriteLine($"Turma '{codigo}' selecionada no dropdown");
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLine($"Erro ao acessar turma: {ex.Message}");
                }
            });
        }
        private void AcessarModulo(Modulo modulo)
        {
            ExecuteInNotasFrame(() =>
            {
                try
                {
                    var nome   = modulo?.Nome ?? string.Empty;
                    var select = new OpenQA.Selenium.Support.UI.SelectElement(
                        Driver.FindElement(By.Id("trimestre")));
                    select.SelectByValue(nome);
                    LogHelper.WriteLine($"Módulo/trimestre '{nome}' selecionado");
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLine($"Erro ao acessar módulo: {ex.Message}");
                }
            });
        }
        private IList<IWebElement> LocalizarParciaisDisponiveis()
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            return wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(
                By.CssSelector(".nota-input")));
        }
        private void ProcessarParcial(Parcial parcial, float notaMinima, float notaMaxima)
        {
            ExecuteInNotasFrame(() =>
            {
                try
                {
                    DigitarNotasParaTodosAlunos(notaMinima, notaMaxima);
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLine($"Erro ao processar parcial: {ex.Message}");
                }
            });
        }

        private void DigitarNotasParaTodosAlunos(float notaMinima, float notaMaxima)
        {
            var inputs = Driver.FindElements(By.CssSelector(".nota-input"));

            for (int i = 0; i < inputs.Count; i++)
            {
                try
                {
                    var current = inputs[i].GetAttribute("value") ?? string.Empty;
                    for (int k = 0; k < Math.Max(current.Length, 4); k++)
                        inputs[i].SendKeys(Keys.Backspace);

                    float valorNota = _random.Next(
                        (int)(notaMinima * 10),
                        (int)(notaMaxima * 10) + 1) / 10f;

                    inputs[i].SendKeys(valorNota.ToString(new CultureInfo("pt-BR")));
                    LogHelper.WriteLine($"  Aluno {i + 1}: nota {valorNota}");
                }
                catch (StaleElementReferenceException)
                {
                    SwitchToDefaultContent();
                    SwitchToNotasFrame();
                    i--;
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLine($"  Erro no aluno {i + 1}: {ex.Message}");
                }
            }
        }
        private void SalvarNotas()
        {
            ExecuteInNotasFrame(() =>
            {
                try
                {
                    Driver.FindElement(By.Id("btn-salvar")).Click();
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLine($"Erro ao clicar em Salvar: {ex.Message}");
                }
            });
        }
    }
}
