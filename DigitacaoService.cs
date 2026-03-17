// AutomacaoIframeDemo/Services/DigitacaoService.cs
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using AutomacaoIframeDemo.Domain;

namespace AutomacaoIframeDemo.Services
{
    public interface IDigitacaoService
    {
        void AdicionarTurmas(List<string> codigosTurmas);
        void AdicionarModulosParaTurmaAtual(List<string> modulos);
        Turma ObterProximaTurmaParaProcessar();
        Turma ObterTurmaAtual();
        void MarcarTurmaComoProcessada();
        List<Turma> ObterTodasTurmas();
        bool ExistemTurmasPendentes();
        void Limpar();
    }

    public class DigitacaoService : IDigitacaoService
    {
        private List<Turma> _turmas = new List<Turma>();
        private Turma _turmaAtual;

        public void AdicionarTurmas(List<string> codigosTurmas)
        {
            foreach (var codigo in codigosTurmas)
            {
                if (!_turmas.Any(t => t.Codigo == codigo))
                {
                    _turmas.Add(new Turma { Codigo = codigo.Trim() });
                }
            }
        }

        public void AdicionarModulosParaTurmaAtual(List<string> modulos)
        {
            if (_turmaAtual != null)
            {
                _turmaAtual.Modulos.Clear();
                _turmaAtual.AdicionarModulos(modulos);
                TestContext.WriteLine($"[DigitacaoService] Módulos adicionados à turma {_turmaAtual.Codigo}: {string.Join(", ", modulos)}");
            }
        }

        public Turma ObterProximaTurmaParaProcessar()
        {
            if (_turmaAtual != null && !_turmaAtual.TodosModulosProcessados() && !_turmaAtual.Processada)
                return _turmaAtual;

            Turma proxima = null;
            if (_turmaAtual != null)
            {
                var idx = _turmas.FindIndex(t => t.Codigo == _turmaAtual.Codigo);
                if (idx >= 0)
                {
                    for (int i = idx + 1; i < _turmas.Count; i++)
                    {
                        if (!_turmas[i].Processada) { proxima = _turmas[i]; break; }
                    }
                }
            }

            if (proxima == null)
                proxima = _turmas.FirstOrDefault(t => !t.Processada);

            _turmaAtual = proxima;
            TestContext.WriteLine(_turmaAtual != null
                ? $"[DigitacaoService] Turma atual: {_turmaAtual.Codigo}"
                : "[DigitacaoService] Nenhuma turma pendente");

            return _turmaAtual;
        }

        public void MarcarTurmaComoProcessada()
        {
            if (_turmaAtual != null)
            {
                var codigo = _turmaAtual.Codigo;
                _turmaAtual.Processada = true;
                TestContext.WriteLine($"✅ Turma '{codigo}' marcada como processada");

                _turmaAtual = _turmas.FirstOrDefault(t => !t.Processada && t.Codigo != codigo);
                TestContext.WriteLine(_turmaAtual != null
                    ? $"[DigitacaoService] Avançando para: {_turmaAtual.Codigo}"
                    : "[DigitacaoService] Sem próximas turmas");
            }
        }

        public Turma ObterTurmaAtual() => _turmaAtual;
        public bool ExistemTurmasPendentes() => _turmas.Any(t => !t.Processada);
        public List<Turma> ObterTodasTurmas() => _turmas;

        public void Limpar()
        {
            _turmas.Clear();
            _turmaAtual = null;
        }
    }
}
