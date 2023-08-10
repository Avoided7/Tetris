using Tetris.UI.Enums;
using Tetris.UI.GameComponents;

GameControllerBuilder builder = new GameControllerBuilder();

builder.SetDifficult(Difficult.Normal);

GameController controller = builder.Build();

controller.Start();