// AutomacaoIframeDemo/Domain/Parcial.cs
using OpenQA.Selenium;

namespace AutomacaoIframeDemo.Domain
{
    public class Parcial
    {
        public string Nome { get; set; }
        public int Numero { get; set; }
        public bool Processada { get; set; }
        public IWebElement Elemento { get; set; }
        public float NotaMinima { get; set; }
        public float NotaMaxima { get; set; }

        // No mock: não há "Arred." nem "Final", mas mantemos a lógica original intacta
        public bool IsArredondamento => Nome == "Arred.";
        public bool IsFinal => Nome == "Final";
    }
}
