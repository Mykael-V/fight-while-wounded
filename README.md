# Fight While Wounded — 0.1.0 experimental

Mod para Mount & Blade II: Bannerlord, compilado contra as DLLs locais da versão 1.4.8.

Permite ao protagonista ser considerado apto para combater com 1 HP ou mais. Não aumenta a vida, não regenera HP e não altera o limite dos outros heróis. Usa o sistema de modelos do jogo; não requer Harmony, ButterLib ou MCM.

## Instalação pelo Vortex

1. Use `FightWhileWounded-0.1.0-bannerlord-1.4.8.zip` na opção de instalar mod a partir de arquivo do Vortex.
2. Ative e faça Deploy.
3. Na ordem de carregamento, coloque Fight While Wounded depois de Sandbox, StoryMode, NavalDLC e dos outros mods de jogabilidade, preferencialmente por último.
4. Confira se está marcado no launcher.
5. Carregue uma cópia do seu save. Ao terminar de carregar, deve aparecer a mensagem de ativação.

Alternativa manual: extraia a pasta `Modules/FightWhileWounded` do ZIP para a pasta `Modules` do jogo. O arquivo final deve ficar em `Modules/FightWhileWounded/SubModule.xml`, sem uma pasta Modules extra dentro de Modules.

## Escopo e compatibilidade

O modelo `PlayerWoundedStatsModel` é registrado através de `AddModel<CharacterStatsModel>`. O jogo inicializa `BaseModel` com o modelo anterior. Vida máxima, tier e limite de ferimento dos demais heróis são delegados ao modelo anterior. O limite do protagonista passa a zero.

Isso muda a classificação de ferimento do protagonista na campanha como um todo, inclusive outros sistemas que consultem `Hero.IsWounded`; não é uma exceção restrita ao botão de batalha. Não altera diretamente o estado de um agente já derrubado em combate. Restrições de prisão, morte e outras regras de entrada continuam existindo.

Ao terminar de carregar a sessão, o mod recalcula os valores em cache do roster que contém o protagonista. `InitializeCachedData` é privado na 1.4.8 e é chamado uma vez via reflection; depois o mod atualiza a versão do roster. Não subtrai manualmente o número de feridos.

Se outro mod registrar um modelo depois e não encadear os modelos anteriores, pode impedir esta alteração. O mod mostra uma mensagem para esse caso; coloque-o por último. A lista completa de mods do usuário não foi auditada.

## Verificação realizada

- Native/SubModule.xml instalado: v1.4.8.
- IL da DLL instalada: `Hero.IsWounded` compara `HitPoints <= WoundedHealthLimit`.
- IL: `Hero.WoundedHealthLimit` chama o modelo ativo `CharacterStatsModel.WoundedHitPointLimit(this)`.
- IL: `DefaultCharacterStatsModel.WoundedHitPointLimit` retorna 20 HP fixos.
- IL: registro genérico de modelo inicializa o modelo anterior antes de registrá-lo.
- IL: o cálculo de feridos do roster usa `Hero.IsWounded`.
- Compilação C# contra os assemblies instalados passou; manifesto XML validado.

**Ainda não testado dentro do jogo.** A versão 0.1.0 é um pacote experimental para validar, não uma release com compatibilidade comprovada. Batalhas terrestres, cercos, batalhas navais, carregar/remover o mod e interação com outros mods requerem teste prático.

## Teste prático antes de publicar

1. Faça uma cópia do save, fora do Ironman.
2. Carregue um personagem com 1–20 HP e confirme que sua vida não aumentou.
3. Ataque um grupo pequeno com tropas saudáveis: confirme que o protagonista e os aliados aparecem e que a vida inicial permanece igual à da campanha.
4. Repita quando o protagonista for o único membro apto do grupo.
5. Confira o contador de feridos e que companheiros com pouca vida seguem feridos.
6. Seja derrubado: confira fim da missão, perdas, captura e vida após a batalha.
7. Teste cerco e batalha naval separadamente.
8. Salve, feche e recarregue. Depois desative o mod e confira que a regra normal retorna.

## Código e compilação

Código em `source/SubModule.cs`. Execute `build.ps1 -GamePath 'caminho do jogo'` no Windows com o compilador C# do .NET Framework e a fachada netstandard instalada. O script escreve apenas nesta pasta, não instala ou modifica o jogo.

Para desinstalar, desative no Vortex e faça Deploy novamente. O mod não grava campos próprios no save. Isso não substitui o teste de remoção num save de cópia.
