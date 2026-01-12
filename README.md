# ShardBorne

Visão Geral do Projeto

ShardBorne é um Action-Platformer 2D com fortes elementos de RPG e Metroidvania Narrativo, desenvolvido pela InLoop Studios. O jogo mergulha os jogadores numa jornada épica através de um mundo fragmentado, onde a exploração, o combate ágil e as escolhas morais se entrelaçam para desvendar um mistério ancestral.

Premissa

O jogador assume o papel de Tharion, o último dos Noxilith, que acorda sem memória num mundo devastado pela "Queda". A sua missão é recuperar a identidade e descobrir a verdade por trás dos eventos cataclísmicos, recolhendo Fragmentos de Memória (Shards) e interagindo com diversas raças antigas. Cada escolha moral feita por Tharion afeta o desenrolar da narrativa e o mundo ao seu redor.Funcionalidades Principais

1. Narrativa e Mundo (Lore)

•
Protagonista Profundo: Tharion, um Noxilith em busca de redenção e autoconhecimento.

•
Universo Rico: Um mundo habitado por quatro raças distintas – Noxilith, Myridian, Drakthar e Lumivernis – cada uma com a sua própria cultura e história.

•
Enredo Dinâmico: Uma estrutura narrativa em Atos, com escolhas morais (Purificar vs. Corromper) que influenciam variáveis do jogo e o desenvolvimento da personagem.

2. Mecânicas de Jogabilidade (Core Mechanics)

•
Movimentação Fluida: Implementação de uma State Machine robusta para controlo preciso e responsivo do jogador.

•
Habilidades Metroidvania: Progressão através do desbloqueio de habilidades essenciais como Pulo Duplo, Dash, Salto e Deslize na Parede.

•
Combate Estratégico: Ataques básicos e Golpes Pesados para quebrar defesas inimigas, complementados por um sistema de Knockback.

•
Recurso "Eco": Uma mecânica central de gestão de recursos, onde o Eco é recolhido para ativar a habilidade de Recuperar Vida (Hold to Heal).

3. Sistemas de RPG e Progressão

•
Diálogos Avançados: Sistema de diálogo baseado em JSON, permitindo conversas ramificadas, escolhas múltiplas e condições específicas.

•
Quests e Variáveis Persistentes: Um sistema de Save que regista o estado do mundo, interações com NPCs e progresso de missões.

•
Árvore de Habilidades (Skill Tree): Um menu visual intuitivo para desbloquear e gerir as habilidades de Tharion.

•
Minijogos: Atividades secundárias, como um minijogo de colheita baseado em timing, que adicionam profundidade e imersão.

4. Inimigos e Bosses

•
IA de Inimigos: Inimigos com máquinas de estados simples para patrulha, deteção e ataque.

•
Boss Fights Épicas: Confrontos memoráveis, como o Guardião de Obsidiana, com múltiplas fases, ataques variados e cutscenes integradas.

5. Aspetos Técnicos

•
Unity Input System: Suporte abrangente para teclado e comando, com separação clara entre controlo de Gameplay e UI.

•
Gestão de Cenas: Transições suaves entre cenários com Fade In/Out e persistência da posição do jogador.

•
Imersão Audiovisual: Integração de luzes 2D (Light2D), Camera Shake com Cinemachine e um gestor de música dinâmico que adapta a banda sonora ao ambiente.

