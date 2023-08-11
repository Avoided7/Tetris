using System.Text;
using Tetris.UI.Enums;
using Tetris.UI.Utilities;

namespace Tetris.UI.GameComponents;
internal partial class GameController
{
  public void Start()
  {
    _outputSource.Write("Loading...");
    
    BuildMap();

    _outputSource.SetColor(Constants.BASE_COLOR);

    _currentFigure = GenerateRandomFigure();
    _nextFigure = GenerateRandomFigure();

    _updateThread.Start();
    _controlThread.Start();
  }

  private void Update()
  {
    try
    {
      if (!TryMoveDownFigure())
      {
        ClearMapFromHorizontalLines();

        _placedFigures += 1;
        _score += _placedFigures;

        _currentFigure = _nextFigure;
        _nextFigure = GenerateRandomFigure();
      }

      PrintGUI();
    }
    catch (Exception ex)
    {
      EndGame();

      _outputSource.Write(ex.Message);
    }
  }

  private void Timer()
  {
    while (true)
    {
      Update();
      Thread.Sleep((int)(_frameTimeInMilliseconds / _gameSpeed));
    }
  }

  private void Input()
  {
    while (true)
    {
      ConsoleKey pressedKey = _inputSource.ReadKey();

      if(pressedKey == _rotateKey)
      {
        TryRotateFigure();
      }

      if(pressedKey == _leftKey)
      {
        TryChangePosition(_currentFigure.HeightPosition, _currentFigure.WidthPosition - 1);
      }

      if(pressedKey == _rightKey)
      {
        TryChangePosition(_currentFigure.HeightPosition, _currentFigure.WidthPosition + 1);
      }
    }
  }

  private bool TryChangePosition(int newHeight, int newWidth)
  {
    lock (_sync)
    {
      int oldHeight = _currentFigure.HeightPosition;
      int oldWidth = _currentFigure.WidthPosition;

      int wall = 1;

      bool isSucceeded = true;

      if (newHeight + _currentFigure.Height > _heightMap ||
         newWidth + _currentFigure.Width > _widthMap - wall ||
         newWidth < wall)
      {
        return false;
      }

      UpdateCurrentFigureOnMap(FigureUpdateType.Clear);

      _currentFigure.HeightPosition = newHeight;
      _currentFigure.WidthPosition = newWidth;

      if (IsCollision())
      {
        _currentFigure.HeightPosition = oldHeight;
        _currentFigure.WidthPosition = oldWidth;

        isSucceeded = false;
      }

      UpdateCurrentFigureOnMap(FigureUpdateType.Set);

      return isSucceeded;
    }
  }

  private bool TryMoveDownFigure()
  {
    bool changedPosition = TryChangePosition(_currentFigure.HeightPosition + 1, _currentFigure.WidthPosition);

    if(!changedPosition && _currentFigure.HeightPosition < 0)
    {
      throw new Exception("Game ended.");
    }

    return changedPosition;
  }

  private bool TryRotateFigure()
  {
    bool isSucceeded = true;
    UpdateCurrentFigureOnMap(FigureUpdateType.Clear);

    _currentFigure.Rotate();

    if(IsCollision())
    {
      _currentFigure.Rotate(false);
      isSucceeded = false;
    }

    UpdateCurrentFigureOnMap(FigureUpdateType.Set);

    return isSucceeded;
  }

  private bool IsCollision()
  {
    int emptyObject = (int)Objects.Empty;

    for (int height = _currentFigure.HeightPosition; height < _currentFigure.HeightPosition + _currentFigure.Height; height++)
    {
      for(int width = _currentFigure.WidthPosition; width < _currentFigure.WidthPosition + _currentFigure.Width; width++)
      {
        if(height < 0)
        {
          continue;
        }

        if (width >= _widthMap || height >= _heightMap)
        {
          return true;
        }

        if (_map[height, width] != emptyObject && _currentFigure[height - _currentFigure.HeightPosition, width - _currentFigure.WidthPosition] != emptyObject)
        {
          return true;
        }
      }
    }
    return false;
  }

  private void UpdateCurrentFigureOnMap(FigureUpdateType updateType)
  {
    int newValue = updateType switch
    {
        FigureUpdateType.Clear => (int)Objects.Empty,
        FigureUpdateType.Set => (int)Objects.Block,
        _ => throw new ArgumentException($"Incorrect argument. {nameof(updateType)}"),
    };

    int figurePositionHeight = _currentFigure.HeightPosition;
    int figurePositionWidth = _currentFigure.WidthPosition;

    int heightFigure = _currentFigure.Height;
    int widthFigure = _currentFigure.Width;

    for (int height = 0; height < heightFigure; height++)
    {
      for (int width = 0; width < widthFigure; width++)
      {
        int correctHeight = figurePositionHeight + height;
        int correctWidth = figurePositionWidth + width;

        if (correctHeight < 0 || _currentFigure[height, width] == 0)
        {
          continue;
        }

        _map[correctHeight, correctWidth] = newValue;
      }
    }
  }

  private void BuildMap()
  {
    for (int height = 0; height < _heightMap; height++)
    {
      for (int width = 1; width < _widthMap - 1; width++)
      {
        _map[height, width] = (int)Objects.Empty;
      }
      _map[height, 0] = (int)Objects.Wall;
      _map[height, _widthMap - 1] = (int)Objects.Wall;
    }
  }

  private void ClearMapFromHorizontalLines()
  {
    for(int height = 0; height < _heightMap; height++)
    {
      bool lineIsFilled = true;
      for (int width = 1; width < _widthMap - 1; width++)
      {
        if(_map[height, width] != (int) Objects.Block)
        {
          lineIsFilled = false;
          break;
        }
      }

      if(!lineIsFilled)
      {
        continue;
      }

      ClearLine(height);
    }
  }

  private void ClearLine(int height)
  {
    for(int currentHeight = height; currentHeight >= 0; currentHeight--)
    {
      for(int currentWidth = 1; currentWidth < _widthMap - 1; currentWidth++)
      {
        if(currentHeight == 0)
        {
          _map[currentHeight, currentWidth] = (int) Objects.Empty;
          continue;
        }

        _map[currentHeight, currentWidth] = _map[currentHeight - 1, currentWidth];
      }
    }
  }

  private string GetMapAsString()
  {
    StringBuilder map = new ();

    for (int height = 0; height < _heightMap; height++)
    {
      for (int width = 0; width < _widthMap; width++)
      {
        switch (_map[height, width])
        {
          case (int)Objects.Empty:
            map.Append(Constants.EMPTY_SYMBOL);
            break;
          case (int)Objects.Wall:
            map.Append(Constants.WALL_SYMBOL);
            break;
          case (int)Objects.Block:
            map.Append(Constants.BLOCK_SYMBOL);
            break;
        }
      }
      map.Append('\n');
    }

    return map.ToString();
  }

  private void PrintGUI()
  {
    _outputSource.Clear();

    foreach(char symbol in GetMapAsString())
    {
      if (symbol == Constants.BLOCK_SYMBOL)
      {
        _outputSource.SetColor(Constants.BLOCK_COLOR);
        _outputSource.WriteSymbol(symbol);
        _outputSource.SetColor(Constants.BASE_COLOR);

        continue;
      }

      _outputSource.WriteSymbol(symbol);
    }

    _outputSource.Write($"\nScore: {_score}\n");
  }

  private void EndGame()
  {
    _controlThread.Interrupt();
    _updateThread.Interrupt();
  }

  // Static
  private static Figure GenerateRandomFigure()
  {
    Figure newFigure = new Figure((FigureType)Random.Shared.Next(Constants.FIGURE_TYPES_COUNT));

    newFigure.HeightPosition = -newFigure.Height;
    newFigure.WidthPosition = 1;

    return newFigure;
  }
}