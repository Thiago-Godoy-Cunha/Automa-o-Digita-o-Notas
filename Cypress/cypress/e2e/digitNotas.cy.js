const modulos = (Cypress.env('TEST_CENARIOS') || '1,2,3').split(',');
const url = Cypress.env('TEST_URL') || 'https://thiaiaiago.github.io/Automa-o-Digita-o-Notas/';
const notaMin = parseFloat(Cypress.env('TEST_MINIMO') || 0);
const notaMax = parseFloat(Cypress.env('TEST_MAXIMO') || 10);

describe('Digitar notas', () => {
  beforeEach(() => {
    cy.visit(url);
  });

  modulos.forEach((modulo) => {
    it(`Lançando notas no Módulo ${modulo}`, () => {
      cy.SelecionarIframe().within(() => {
        cy.get('#trimestre', { timeout: 40000 })
          .select(`${modulo}º Trimestre`);
        cy.forEachElement('//td/input[contains(@class, "nota")]', ($elNotaAlunos) => {
          const formatoBR = new Intl.NumberFormat('pt-BR');
          const nota = (
            Math.random() * (notaMax - notaMin) + notaMin
          ).toFixed(1);
          cy.wrap($elNotaAlunos).type(formatoBR.format(nota));
        });
        cy.get('#btn-salvar').click();
      });
    });
  });
});