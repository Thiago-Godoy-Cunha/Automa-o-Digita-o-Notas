const modulos = [ '1', '2', '3' ];

describe('Digitar notas', () => {
  before(() => {
    cy.visit('https://thiaiaiago.github.io/Automa-o-Digita-o-Notas/')
  })

  modulos.forEach((modulo) => {
    it(`Lançando notas no Módulo ${modulo}`, () => {      
      cy.SelecionarIframe().within(() => { 
        cy.get('#trimestre', {timeout:40000}).select(`${modulo}º Trimestre`); 
        cy.forEachElement('//td/input[contains(@class, "nota")]', ($elNotaAlunos) => { 
          const formatoBR = new Intl.NumberFormat('pt-BR');
          var nota = (Math.random() * 10).toFixed(1)
          cy.wrap($elNotaAlunos).type(`${formatoBR.format(nota)}`)
          cy.get('#btn-salvar').click(); 
        });
      });
    });  
  });
})