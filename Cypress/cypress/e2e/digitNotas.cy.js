import {login} from '../../login'

describe('Digitar notas', () => {
  beforeEach(() =>{
    login('FF', '#Acesso123', 'https://gvdigital02-dev.professor.gvdasa.com.br')
  });

  it('Pula o iframe e acessa a página de notas diretamente', () => {
  AcessarDigitacaoDeNotas(172);

  cy.iframe('#note-typing-iframe', { timeout: 40000 })
    .contains('p', 'Legenda', { timeout: 30000 })
    .click()
  });
  // Criar função loopando digitação
  
})

function AcessarDigitacaoDeNotas(turma){
    cy.xpath(`//div[text()='${turma}']/../div/div/button[text()='NO']`).click()
}
