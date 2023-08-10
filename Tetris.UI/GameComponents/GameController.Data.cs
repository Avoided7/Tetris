using Tetris.UI.Implementation;
using Tetris.UI.Interfaces;

namespace Tetris.UI.GameComponents;
internal partial class GameController
{
  // Game Settings
  private readonly float _gameSpeed = 1f;
  private readonly int _frameTimeInMilliseconds = 100;

  // Input & Output Source
  private readonly IInputSource _inputSource;
  private readonly IOutputSource _outputSource;

  // Map Settings
  private readonly int _heightMap = 15;
  private readonly int _widthMap = 10;
  private readonly int[,] _map;

  // Threads
  private Thread _updateThread = null!;
  private Thread _controlThread = null!;

  // Figures
  private Figure _currentFigure;
  private Figure _nextFigure;

  // Score & Data
  private int _score;
  private int _placedFigures;

  // Control Keys
  private readonly ConsoleKey _rotateKey = ConsoleKey.R;
  private readonly ConsoleKey _leftKey = ConsoleKey.LeftArrow;
  private readonly ConsoleKey _rightKey = ConsoleKey.RightArrow;

  public GameController(
    float gameSpeed = 1f,
    int height = 15,
    int width = 10,
    IOutputSource outputSource = null!,
    IInputSource inputSource = null!) : this(outputSource, inputSource, height, width)
  {
    _gameSpeed = gameSpeed;
  }

  private GameController(
    IOutputSource outputSource,
    IInputSource inputSource,
    int height,
    int width)
  {
    _updateThread = new Thread(() => Timer());
    _controlThread = new Thread(() => Input());

    if (outputSource is not null)
    {
      _outputSource = outputSource;
    }
    else
    {
      _outputSource = new ConsoleOutput();
    }

    if (inputSource is not null)
    {
      _inputSource = inputSource;
    }
    else
    {
      _inputSource = new ConsoleInput();
    }

    _heightMap = height;
    _widthMap = width;

    _map = new int[height, width];
  }
}
