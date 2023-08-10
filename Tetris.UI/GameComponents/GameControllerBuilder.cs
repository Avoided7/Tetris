using Tetris.UI.Enums;
using Tetris.UI.Implementation;
using Tetris.UI.Interfaces;

namespace Tetris.UI.GameComponents;

internal class GameControllerBuilder
{
  private float _gameSpeed = 1f;
  private IOutputSource _outputSource = new ConsoleOutput();
  private IInputSource _inputSource = new ConsoleInput();
  public GameControllerBuilder SetDifficult(Difficult difficult = Difficult.Normal)
  {
    switch (difficult)
    {
      case Difficult.Easy:
        _gameSpeed = 0.5f;
        break;
      case Difficult.Normal:
        _gameSpeed = 1f;
        break;
      case Difficult.Hard:
        _gameSpeed = 2f;
        break;
      default:
        throw new ArgumentException("Incorrect enum value.");
    }
    return this;
  }

  public GameControllerBuilder SetOutputSource(IOutputSource outputSource)
  {
    ArgumentException.ThrowIfNullOrEmpty(nameof(outputSource));
    _outputSource = outputSource;
    return this;
  }

  public GameControllerBuilder SetInputSource(IInputSource inputSource)
  {
    ArgumentException.ThrowIfNullOrEmpty(nameof(inputSource));
    _inputSource = inputSource;
    return this;
  }

  public GameController Build()
  {
    return new GameController(_gameSpeed, outputSource: _outputSource, inputSource: _inputSource);
  }
}
