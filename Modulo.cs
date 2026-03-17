// AutomacaoIframeDemo/Domain/Modulo.cs
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace AutomacaoIframeDemo.Domain
{
    public class Modulo
    {
        public string Nome { get; set; }
        public List<Parcial> Parciais { get; set; } = new List<Parcial>();
        public bool Processado { get; set; }
        public bool DisponivelParaAlteracao { get; set; }
        public IWebElement ElementoSeletor { get; set; }

        public Parcial ObterParcialAtual()
        {
            return Parciais.FirstOrDefault(p => !p.Processada);
        }

        public bool TodasParciaisProcessadas()
        {
            return Parciais.All(p => p.Processada) || Parciais.Count == 0;
        }

        /// <summary>
        /// No mock, as parciais são os inputs de nota de cada aluno (.nota-input).
        /// Adaptado do original que usava XPath para extrair nome de ancestor::th.
        /// </summary>
        public void AdicionarParciais(List<IWebElement> elementosParciais)
        {
            for (int i = 0; i < elementosParciais.Count; i++)
            {
                Parciais.Add(new Parcial
                {
                    Nome = $"Parcial {i + 1}",
                    Numero = i + 1,
                    Elemento = elementosParciais[i]
                });
            }
        }
    }
}
