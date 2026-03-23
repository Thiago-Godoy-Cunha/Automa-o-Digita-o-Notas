#language: pt

@digitacao
Funcionalidade: Digitar Notas
    Sendo um professor logado
    Quero acessar as notas de minhas turmas
    Para que eu possa adicionar notas das avaliações

    Esquema do Cenário: Acessar digitacao de notas
        Dado que estou logado no portal de demonstração
        E que acesso a página de Notas da turma "<input_turma>"
        Quando seleciono módulo "<input_modulo>"
        Então devo ver o módulo selecionado disponível para alteração
        Quando digito notas aleatoriamente num range de "<minimoRandNotas>" a "<maximoRandNotas>" na "<inputParcial>"° parcial

        Exemplos:
        | input_turma | input_modulo | minimoRandNotas | maximoRandNotas | inputParcial |
        | 9A          | T1           | 4               | 9               | 1            |
