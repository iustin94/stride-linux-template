using StrideTemplate.Core;
using StrideTemplate.UI;
using Stride.CommunityToolkit.Engine;
using Stride.Engine;
using Stride.Games;

using var game = new Game();
GameManager? gameManager = null;

game.Run(null, Start, Update);

void Start(Scene rootScene)
{
    game.SetupBase3D();
    UIHelper.Initialize(game);

    var sceneManager = new SceneManager(rootScene, game.Services);
    gameManager = new GameManager(sceneManager, game, rootScene);

    gameManager.GoToMainMenu();
}

void Update(Scene rootScene, GameTime time)
{
    gameManager?.Update(time);
}
