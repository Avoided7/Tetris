using System.Text;
using Tetris.UI.Enums;
using Tetris.UI.Utilities;

namespace Tetris.UI.GameComponents;
internal partial class GameController
{
  public void Start()
  {
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
        _outputSource.Write("Rotated!");
        _currentFigure.Rotate();
      }

      if(pressedKey == _leftKey)
      {
        TryChangePosition(_currentFigure.Height, _currentFigure.Width - 1);
      }

      if(pressedKey == _rightKey)
      {
        TryChangePosition(_currentFigure.Height, _currentFigure.Width + 1);
      }
    }
  }

  private bool TryChangePosition(int newHeight, int newWidth)
  {
    int oldHeight = _currentFigure.Height;
    int oldWidth = _currentFigure.Width;

    int wall = 1;

    if(newHeight + _currentFigure.Volume.GetLength(0) > _heightMap ||
       newWidth + _currentFigure.Volume.GetLength(1) > _widthMap - wall || 
       newWidth < wall)
    {
      return false;
    }

    UpdateCurrentFigureOnMap(FigureUpdateType.Clear);

    _currentFigure.Height = newHeight;
    _currentFigure.Width = newWidth;

    if(IsCollision())
    {
      _currentFigure.Height = oldHeight;
      _currentFigure.Width = oldWidth;

      UpdateCurrentFigureOnMap(FigureUpdateType.Set);

      return false;
    }

    UpdateCurrentFigureOnMap(FigureUpdateType.Set);

    return true;
  }

  private bool TryMoveDownFigure()
  {
    bool changedPosition = TryChangePosition(_currentFigure.Height + 1, _currentFigure.Width);

    if(!changedPosition && _currentFigure.Height < 0)
    {
      throw new Exception("Game ended.");
    }

    return changedPosition;
  }

  private bool IsCollision()
  {
    int emptyObject = (int)Objects.Empty;

    for (int height = _currentFigure.Height; height < _currentFigure.Height + _currentFigure.Volume.GetLength(0); height++)
    {
      for(int width = _currentFigure.Width; width < _currentFigure.Width + _currentFigure.Volume.GetLength(1); width++)
      {
        if(height < 0)
        {
          continue;
        }

        if (_map[height, width] != emptyObject && _currentFigure.Volume[height - _currentFigure.Height, width - _currentFigure.Width] != emptyObject)
        {
          return true;
        }
      }
    }
    return false;
  }

  private void UpdateCurrentFigureOnMap(FigureUpdateType updateType)
  {
    int newValue = -1;

    switch(updateType)
    {
      case FigureUpdateType.Clear:
        newValue = (int)Objects.Empty;
        break;
      case FigureUpdateType.Set:
        newValue = (int)Objects.Block;
        break;
      default:
        throw new ArgumentException($"Incorrect argument. {nameof(updateType)}");
    }

    int figurePositionHeight = _currentFigure.Height;
    int figurePositionWidth = _currentFigure.Width;

    int heightFigure = _currentFigure.Volume.GetLength(0);
    int widthFigure = _currentFigure.Volume.GetLength(1);

    for (int height = 0; height < heightFigure; height++)
    {
      for (int width = 0; width < widthFigure; width++)
      {
        int correctHeight = figurePositionHeight + height;
        int correctWidth = figurePositionWidth + width;

        if (correctHeight < 0 || _currentFigure.Volume[height, width] == 0)
        {
          continue;
        }

        _map[correctHeight, correctWidth] = newValue;
      }
    }
  }

  private Figure GenerateRandomFigure()
  {
    Figure newFigure = new Figure((FigureType)Random.Shared.Next(Constants.FIGURE_TYPES_COUNT));

    newFigure.Height = -newFigure.Volume.GetLength(0);
    newFigure.Width = 1;

    return newFigure;
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

  private string GetMapAsString()
  {
    StringBuilder map = new StringBuilder();

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
      map.Append("\n");
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
}