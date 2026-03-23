// AutomacaoIframeDemo/Domain/Turma.cs
using System.Collections.Generic;
using System.Linq;

namespace AutomacaoIframeDemo.Domain
{
    public class Turma
    {
        public string Codigo { get; set; }
        public List<Modulo> Modulos { get; set; } = new List<Modulo>();
        public bool Processada { get; set; }
        public string UrlNotas { get; set; }

        public Modulo ObterModuloAtual()
        {
            return Modulos.FirstOrDefault(m => !m.Processado);
        }

        public bool TodosModulosProcessados()
        {
            return Modulos.All(m => m.Processado) || Modulos.Count == 0;
        }

        public void AdicionarModulos(List<string> nomesModulos)
        {
            foreach (var nome in nomesModulos)
            {
                Modulos.Add(new Modulo { Nome = nome });
            }
        }
    }
}
