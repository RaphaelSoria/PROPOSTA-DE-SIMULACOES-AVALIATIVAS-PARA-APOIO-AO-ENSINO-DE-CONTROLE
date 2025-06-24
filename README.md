# PROPOSTA-DE-SIMULACOES-AVALIATIVAS-PARA-APOIO-AO-ENSINO-DE-CONTROLE
Este repositório contém o código-fonte e os recursos do Trabalho de Conclusão de Curso (TCC) de Raphael Soria de Avila, para o curso de Engenharia Mecatrônica do Centro Tecnológico de Joinville (CTJ) da Universidade Federal de Santa Catarina (UFSC).

--Sobre o Projeto
O ensino de engenharia de controle frequentemente enfrenta o desafio de conectar conceitos teóricos abstratos com a aplicação prática. Métodos tradicionais, baseados em aulas expositivas e memorização de técnicas, criam uma lacuna entre a teoria e a prática que pode desestimular os estudantes.

Este projeto busca endereçar essa questão através do desenvolvimento de uma metodologia de ensino baseada em simulação (SBT). Foi criada uma simulação interativa do sistema barra-bola, um problema canônico da teoria de controle , utilizando o motor de desenvolvimento Unity. A ferramenta permite que estudantes projetem, sintonizem e validem um controlador Proporcional-Integral-Derivativo (PID)  em um ambiente virtual seguro, acessível e visualmente intuitivo.


O objetivo é oferecer um laboratório virtual que não apenas demonstra a teoria, mas serve como uma plataforma de experimentação e avaliação, preenchendo a lacuna entre a sala de aula e os desafios práticos da engenharia.


--Principais Funcionalidades
Simulação em Tempo Real: Comportamento dinâmico do sistema barra-bola representado através do motor de física PhysX integrado ao Unity.
Controlador PID Interativo: Os alunos podem inserir os ganhos Kp, Ki e Kd diretamente na interface e observar os efeitos em tempo real sobre a estabilidade do sistema.

Ambiente Configurável pelo Professor: Permite que o docente defina os parâmetros do desafio (posição inicial, final, tolerância, etc.) através de um arquivo de configuração externo no formato .json.

Validação Automática: O sistema avalia objetivamente se o controle foi bem-sucedido com base em critérios de estabilidade e tempo de acomodação.
Coleta de Dados para Análise: Em caso de falha ou sucesso, a simulação gera e armazena um log com os dados de erro e desempenho para uma análise quantitativa posterior pelo professor.


--Tecnologias Utilizadas

Motor de Desenvolvimento: Unity 3D
Linguagem de Programação: C# 
Motor de Física: NVIDIA PhysX (integrado ao Unity) 

--Como Utilizar

-Pré-requisitos
Unity Hub
Editor Unity (6000.1.1f1)

-Instalação e Execução
Clone o repositório:
git clone https://github.com/[SEU-USUARIO]/[NOME-DO-REPOSITORIO].git

-Abra o projeto no Unity Hub:
No Unity Hub, clique em "Open" -> "Add project from disk".
Navegue até a pasta do projeto clonado e selecione-a.
Abra a Cena Principal:
Na janela "Project" do Unity, navegue até a pasta Assets/Scenes.
Abra a cena principal da simulação (ex: CenaPrincipal.unity).
Execute a Simulação:
Pressione o botão ▶ (Play) no topo da interface do editor.
Configuração da Atividade
A simulação é configurada por meio de um arquivo config.json, permitindo ao professor criar diferentes desafios.

Exemplo de config.json:

{
  "Posicao_Inicial": -1.0,
  "Posicao_Final": 0.0,
  "Tolerancia": 0.05,
  "tempoMaximo_Estabilizacao": 15.0
}

Posicao_Inicial: Ponto de partida da bola na barra.
Posicao_Final: Alvo (setpoint) que o controlador deve alcançar.
Tolerancia: Faixa de erro permitida em torno da posição final para considerar o sistema estável.
tempoMaximo_Estabilizacao: Tempo máximo em segundos para que o sistema estabilize dentro da tolerância.


Licença

Distribuído sob a licença MIT. Veja LICENSE.txt para mais informações.
