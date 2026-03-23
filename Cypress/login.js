/// <reference types="cypress" />
require('@cypress/xpath');

Cypress.on('uncaught:exception', (err, runnable) => {
  console.error('Erro não capturado:', err);
  return false;
});
 
// Função de login reutilizável
export const login = (login, senha, url) => {
  cy.visit(url);
  cy.origin('https://gvdigital.gvdasa.com.br', { args: { login, senha } },  ({ login, senha }) => {
    cy.get('input[name="UserName"]').type(login);
    cy.get('input[name="Password"]').should('be.visible').type(senha);
    cy.get('button[type="submit"]').should('be.visible').click();
  })
    cy.xpath('//h6[text()="Professor"]/../button').click();
    cy.xpath('//p[text()="Avançar"]').click();
};