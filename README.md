# Automação de Digitação de Notas — iFrame com Selenium e Cypress

Projeto de automação de testes cobrindo um fluxo de digitação de notas escolares em uma aplicação web que utiliza `<iframe>` para compartilhar componentes entre páginas.

O mesmo fluxo foi implementado em **duas stacks diferentes** — Selenium WebDriver (C#) e Cypress (JavaScript) — com abordagens distintas para o problema central: interagir com elementos dentro de um iframe.

🔗 **Demo (GitHub Pages):** [https://thiaiaiago.github.io/Automa-o-Digita-o-Notas/](https://thiaiaiago.github.io/Automa-o-Digita-o-Notas/)

---

## O problema

A aplicação reutiliza o módulo de lançamento de notas em diferentes portais via `<iframe>`. Isso cria um desafio direto para automação de testes: as ferramentas precisam alternar o contexto de execução para dentro do frame antes de interagir com seus elementos.

Cada ferramenta resolve esse problema de forma diferente — e este projeto demonstra as duas abordagens lado a lado.

---

## Implementação 1 — Selenium WebDriver (C# + SpecFlow)

### Como funciona

O Selenium resolve o problema nativamente via `SwitchTo().Frame()`, que alterna o contexto do WebDriver para dentro do iframe. Todos os `FindElement` subsequentes passam a buscar dentro do frame. Após a interação, `SwitchTo().DefaultContent()` retorna ao contexto principal.

```csharp
// BaseStepDef.cs
protected void SwitchToNotasFrame()
{
    var iframe = WaitForElement(By.CssSelector("#notas-frame"));
    Driver.SwitchTo().Frame(iframe);
}

protected void ExecuteInNotasFrame(Action action)
{
    SwitchToNotasFrame();
    try   { action(); }
    finally { Driver.SwitchTo().DefaultContent(); }
}
```

O padrão `ExecuteInNotasFrame` garante que o WebDriver sempre retorna ao contexto principal, mesmo em caso de exceção — evitando que testes subsequentes falhem por contexto desatualizado.

### Stack

- **C#** — mesma linguagem do time de desenvolvimento, facilitando comunicação e pedidos de ajuda
- **SpecFlow** — BDD com cenários em português e tabelas de exemplos parametrizáveis
- **Selenium WebDriver** — suporte nativo a troca de contexto de iframe
- **GitHub Actions** — pipeline com `workflow_dispatch` e parâmetros configuráveis (turma, trimestre, parcial, range de notas)

### Estrutura

```
/selenium
  /Features
    entrarEmNotasDaTurma.feature   # Cenários BDD em português
  /Steps
    BaseStepDef.cs                 # SwitchTo().Frame() centralizado aqui
    DigitNotasStepDef.cs           # Fluxo completo de digitação
  /Domain
    Turma.cs · Modulo.cs · Parcial.cs
  /Services
    DigitacaoService.cs            # Orquestra turmas e módulos
  /Contexts
    SharedScenarioContext.cs       # Setup/teardown do WebDriver
  /Hooks
    Hooks.cs                       # Logs por cenário via NUnit
```

### Rodando localmente

```bash
# Subir o mock app
npx http-server ./mock-app -p 5500

# Rodar os testes
BASE_URL=http://localhost:5500/index.html dotnet test --filter "TestCategory=digitacao"
```

---

## Implementação 2 — Cypress (JavaScript + cypress-iframe)

### Como funciona

O Cypress não suporta iframes nativamente por limitações do seu modelo de execução baseado em JavaScript. A solução foi o plugin `cypress-iframe`, que expõe o comando `cy.iframe()` para selecionar o frame e `.within()` para escopar as ações dentro dele.

Um custom command encapsula a lógica de seleção do iframe, mantendo os testes limpos:

```javascript
// commands.js
Cypress.Commands.add('SelecionarIframe', () => {
  return cy.iframe('#notas-frame', { timeout: 40000 });
});
```

```javascript
// digitNotas.cy.js
cy.SelecionarIframe().within(() => {
  cy.get('#trimestre').select(`${modulo}º Trimestre`);
  cy.forEachElement('//td/input[contains(@class, "nota")]', ($el) => {
    const nota = (Math.random() * 10).toFixed(1);
    cy.wrap($el).type(`${new Intl.NumberFormat('pt-BR').format(nota)}`);
  });
  cy.get('#btn-salvar').click();
});
```

Um segundo custom command `forEachElement` combina XPath (via `@cypress/xpath`) com `.each()` para iterar sobre todos os inputs de nota de forma declarativa.

### Stack

- **JavaScript** — linguagem nativa do Cypress
- **Cypress** — ferramenta padrão de automação web
- **cypress-iframe** — plugin para interação com iframes
- **@cypress/xpath** — suporte a seletores XPath no Cypress

### Estrutura

```
/cypress
  /e2e
    digitNotas.cy.js              # Spec de digitação de notas
  /support
    commands.js                   # Custom commands: SelecionarIframe, forEachElement
    e2e.js                        # Setup global: imports de plugins
```

### Rodando localmente

```bash
# Instalar dependências
npm install

# Abrir Cypress em modo interativo
npx cypress open

# Ou rodar headless
npx cypress run
```

---

## Comparativo das abordagens

| | Selenium (C#) | Cypress (JS) |
|---|---|---|
| **Suporte nativo a iframe** | ✅ Via `SwitchTo().Frame()` | ❌ Requer plugin `cypress-iframe` |
| **Troca de contexto** | Explícita — `SwitchTo` / `DefaultContent` | Implícita — escopo via `.within()` |
| **Linguagem** | C# | JavaScript |
| **BDD** | SpecFlow + Gherkin | — |
| **Pipeline configurável** | GitHub Actions com `workflow_dispatch` | — |
| **Curva de aprendizado** | Maior | Menor |
| **Velocidade de execução** | Mais lento | Mais rápido |

### Quando usar cada um

**Selenium** faz mais sentido quando o time já usa C#/.NET, quando há necessidade de BDD com cenários legíveis por pessoas não técnicas, ou quando a pipeline precisa de parâmetros configuráveis por execução sem edição de código.

**Cypress** faz mais sentido quando o time já usa JavaScript, quando a prioridade é velocidade de desenvolvimento e feedback rápido, ou quando o projeto de automação precisa de menor complexidade de setup.

---

## Mock App

A aplicação de demonstração simula um portal escolar com um módulo de lançamento de notas carregado via iframe. Está hospedada no GitHub Pages e pode ser usada como alvo de testes sem nenhuma configuração adicional.

**Funcionalidades do mock:**
- Seleção de turma e trimestre via `<select>`
- Tabela de alunos com inputs de nota e faltas por trimestre
- Cálculo automático de média final
- Botão de salvar com confirmação visual
- Validação de range de notas configurável

---

## Sobre

Projeto desenvolvido como portfólio demonstrativo. A arquitetura e os casos de teste refletem um problema real de automação enfrentado em ambiente de produção — cobertura de telas compartilhadas via iframe em um sistema de gestão escolar — adaptado com domínio e dados fictícios para publicação pública.
