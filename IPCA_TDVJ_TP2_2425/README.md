# IPCA_TDVJ_TP2_2425

# IPCA_TDVJ_TP2_2425

# Trabalho prático - Técnicas de desenvolvimento de videojogos

David Querido nº 33219

Gabriel Solinos nº 31487

Paulo Pinto nº 31474

## Análise do jogo "Zelda"

O jogo Zelda é um jogo bidimensional de aventura com vista superior, desenvolvido com a framework MonoGame. O principal objetivo do jogador é explorar um mapa labiríntico, composto por vários ecrãs, enfrentando inimigos e ultrapassando obstáculos. O jogador controla uma personagem que pode mover-se em quatro direcções (cima, baixo, esquerda e direita) utilizando as teclas direcionais ou as teclas WASD. Além disso, o jogador pode disparar projécteis para eliminar inimigos, recorrendo à tecla Espaço. O jogo conta com inimigos controlados por uma inteligência artificial simples: estes perseguem o jogador sempre que se encontram no mesmo ecrã, ajustando automaticamente a sua posição para tentar interceptá-lo. Caso o jogador colida com um inimigo, o jogo termina imediatamente. No ambiente de jogo, o jogador pode encontrar paredes e caixas. As caixas podem ser empurradas, desde que exista espaço livre à sua frente, permitindo ao jogador abrir caminho e resolver pequenos desafios de movimentação. O mapa do jogo é maior do que o ecrã visível, pelo que o jogador vai explorando diferentes áreas conforme se desloca. O jogo termina quando todos os inimigos forem eliminados (vitória do jogador) ou se o jogador for apanhado por um inimigo (derrota). Não existe um sistema de pontuação, vidas múltiplas ou tempo limite: a experiência baseia-se na sobrevivência e na exploração.

## Progressão no jogo

No início do jogo, o jogador aparece numa posição definida do mapa. O mundo é composto por vários ecrãs ligados entre si, que o jogador explora ao deslocar-se para as extremidades de cada ecrã. A progressão faz-se através da exploração destes ecrãs, procurando inimigos para eliminar e caminhos livres para avançar. Ao longo do percurso, o jogador encontra obstáculos como paredes e caixas. As caixas podem ser empurradas para abrir caminho, desde que exista espaço suficiente, criando pequenos desafios de raciocínio e movimentação. Em cada ecrã podem surgir inimigos que, ao detetarem o jogador, começam a persegui-lo. O jogador deve evitar o contacto direto com os inimigos, utilizando os projécteis para os eliminar à distância. A condição de vitória é clara: eliminar todos os inimigos presentes no mapa. Assim que o último inimigo for destruído, o jogo termina e o jogador vence. Por outro lado, se o jogador colidir com um inimigo, o jogo termina imediatamente, resultando na derrota. Durante o jogo, não existe um sistema de níveis, pontos ou vidas extra. A progressão é linear e baseada na capacidade do jogador para sobreviver, explorar e derrotar todos os inimigos. O feedback do jogo é imediato, com o fecho automático da aplicação assim que se verifica a condição de vitória ou derrota.

## Comandos para o jogo

- **Mover para a direita** – Seta para a direita ou "D";
- **Mover para a esquerda** – Seta para a esquerda ou "A";
- **Mover para cima** - Seta para cima ou "W";
- **Mover para baixo** - Seta para baixo ou "S";
- **Disparar projétil** – Tecla "Espaço".
- **Aumentar o volume** - Tecla "Q";
- **Diminuir o volume** - Tecla "E";
- **Sair do jogo** - Tecla "ESC";

## Estrutura das pastas

- `Content/` – Contém todos os recursos do jogo(imagens, sons, mapas, etc.) que serão compilados e usados no jogo:
  - `player.png` - Representa a imagem do nosso personagem;
  - `enemy.png` - Representa a imagem dos nossos inimigos(todos eles são do mesmo tipo);
  - `projectile.png` – Representa a sprite para a bola de fogo que é o objeto atirado pelo Player;
  - `map.txt` – Contém a descrição do mapa do jogo( é uma matriz com a indicação da posição dos obstáculos, os inimigos e o player, etc.);
  - `thornfloor damp.png` – Representa a textura do solo do mapa;
  - `Content.mgcb` – É o ficheiro de build do conteúdo do MonoGame;
  - `sound.mp3`- Representa a música de fundo.

## Classes

### `Game1.cs`:
A classe Game1 é a principal do jogo, sendo responsável por todo o seu ciclo de vida. Herda de Game e gere a inicialização, carregamento de conteúdos, atualização da lógica e renderização dos gráficos no ecrã. Controla o mapa e as entidades, garantindo o carregamento, atualização e desenho do jogador, inimigos, caixas e outros objetos. Também lida com a entrada do utilizador, capturando comandos do teclado para pausar, sair ou ajustar o volume da música. Além disso, gere a trilha sonora, carregando e ajustando a música de fundo. A classe implementa a inicialização dos gráficos e do ecrã através do GraphicsDeviceManager, carrega texturas para os elementos do jogo e interpreta o mapa a partir de um ficheiro de texto, identificando diferentes caracteres para o chão, a parede, os inimigos e o jogador. No que toca à gestão de entidades, instancia e armazena referências ao jogador (Player) e aos inimigos (Enemy). A lógica de atualização (Update) processa entradas do teclado, atualiza a posição do jogador, inimigos e projéteis, verifica colisões entre projéteis e inimigos e controla a transição de áreas do mapa. O jogo termina se todos os inimigos forem derrotados. Na renderização (Draw), desenha piso, paredes, jogador, inimigos e projéteis, ajustando tudo conforme a posição do jogador. Também inclui funções utilitárias como FreeTile para verificar a presença de espaços livres, CreateColoredTexture para gerar texturas simples, LoadMap para interpretar o mapa e inicializar objetos. Em resumo, a classe Game1 é o núcleo do jogo, coordenando todos os elementos essenciais para o seu funcionamento.

### `Player.cs`:
A classe Player representa o jogador no jogo Zelda desenvolvido com MonoGame, sendo responsável por toda a lógica do personagem controlado pelo jogador. Gere a posição tanto em coordenadas de tile como de pixel, permitindo um movimento suave baseado na velocidade e no tempo decorrido, com suporte para quatro direções e normalização do movimento diagonal. Também trata colisões com obstáculos e permite empurrar caixas caso o caminho esteja livre. Utiliza uma spritesheet para animar o personagem conforme a direção e o estado de movimento, atualizando o quadro da animação conforme necessário. Destaca-se pela mecânica de disparo de projéteis, lançados na direção em que o jogador está a olhar, respeitando um tempo de recarga entre disparos. Os projéteis são armazenados e atualizados a cada frame. Os métodos principais incluem Update, que atualiza posição, animação, projéteis e interações; Draw, que desenha o jogador com o quadro correto; DrawProjectiles, que desenha os projéteis disparados; e métodos para carregar as texturas do jogador e dos projéteis. Assim, esta classe centraliza todo o comportamento do personagem principal, incluindo movimentação, animação, interação com o cenário e ataques, sendo essencial para o funcionamento do jogo.

### `Enemy.cs`:
A classe Enemy representa os inimigos dentro do jogo, sendo responsável por controlar a sua posição no mapa, o seu movimento em direção ao jogador, a animação dos seus sprites conforme a direção em que está a deslocar-se e também por desenhá-lo no ecrã. Mantém informações como a posição do inimigo em tiles (blocos do mapa) e em pixels (para uma animação suave), além de armazenar referências à spritesheet e aos quadros de animação. A função LoadContent carrega a imagem da spritesheet do inimigo e divide essa imagem em vários quadros de animação, assumindo um esquema de 4x4 (quatro direções e quatro quadros por direção). No método Update, o inimigo só se move caso esteja no mesmo ecrã que o jogador. Determina a direção mais curta até ao jogador, ajusta a sua posição caso o tile de destino esteja livre e atualiza o quadro da animação enquanto estiver a mover-se. Se não estiver a movimentar-se, a animação é reiniciada. Se o inimigo encostar no jogador, o jogo termina imediatamente. Por fim, o método Draw desenha o inimigo no ecrã usando o quadro de animação correto e ajustando a posição conforme o deslocamento do ecrã, garantindo que o inimigo apareça corretamente mesmo com o movimento da câmara.

### `Projectile.cs`:
A classe Projectile representa o projétil no jogo, neste caso, uma bola de fogo. Gere toda a sua lógica, desde a criação até deixar de ser relevante. Os principais atributos incluem posição, direção, velocidade, estado ativo, referência ao objeto principal do jogo e a textura para o desenhar no ecrã.O construtor inicializa o projétil com a posição de origem, direção, velocidade e uma referência ao jogo, marcando-o como ativo. Há métodos para carregar a textura, atualizar a posição com base na direção, velocidade e tempo, e verificar se saiu dos limites do mapa, desativando-o se necessário. O método Draw desenha o projétil no ecrã, ajustando a rotação e o espelhamento conforme a direção. Usa GetDirectionIndex, que converte a direção num índice para definir corretamente o efeito gráfico. O método GetBounds retorna o retângulo de colisão do projétil, essencial para detetar colisões. Em resumo, a classe Projectile encapsula toda a lógica dos projéteis no jogo, facilitando a sua movimentação, desenho e deteção de colisões para uma gestão eficiente em diversas situações.

### `Program.cs`:
Ponto de entrada da aplicação, executa a classe `Game1`.

## Melhorias futuras:

- Adicionar o ecrã inicial e uma tela de fim de jogo, indicando vitória ou derrota;
- Acrescentar um sistema de saúde/pontuação visível;
- Desenvolver a documentação do input do jogador;
- Adicionar mais niveis para o jogo;
- Implementação de um maior número de vidas, tanto para o jogador como para os inimigos para tornar o jogo mais dinâmico;
- Criar uma animação para idle.

