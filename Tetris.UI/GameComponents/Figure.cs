using Tetris.UI.Enums;
using Tetris.UI.Utilities;

namespace Tetris.UI.GameComponents;
internal struct Figure
{
  public int HeightPosition { get; set; }
  public int WidthPosition { get; set; }
  public int Height => _volume.GetLength(0);
  public int Width => _volume.GetLength(1);
  
  private int[,] _volume = null!;

  public int this[int height, int width]
  {
    get
    {
      return _volume[height, width];
    }
    set
    {
      _volume[height, width] = value;
    }
  }

  public Figure(int height, int width, FigureType figureType = FigureType.Block) : this (figureType)
  {
    HeightPosition = height;
    WidthPosition = width;
  }
  public Figure(FigureType figureType = FigureType.Block)
  {
    SetVolume(figureType);
  }

  public void Rotate(bool byClockwise = true)
  {
    int[,] newFigure = new int[Width, Height];

    if (byClockwise)
    {
      for(int height = 0; height < Height; height++)
      {
        for(int width = 0; width < Width; width++)
        {
          newFigure[width, height] = _volume[Height - height - 1, width];
        }
      }

      _volume = newFigure;

      return;
    }

    for (int height = 0; height < Height; height++)
    {
      for (int width = 0; width < Width; width++)
      {
        newFigure[width, height] = _volume[height, Width - width - 1];
      }
    }

    _volume = newFigure;

    return;
  }

  private void SetVolume(FigureType figureType)
  {
    switch (figureType)
    {
      case FigureType.Block:
        _volume = Constants.Block;
        break;
      case FigureType.Line:
        _volume = Constants.Line;
        break;
      case FigureType.ObjectL:
        _volume = Constants.ObjectL;
        break;
      default:
        throw new ArgumentException($"Incorrect argument. {nameof(figureType)}");
    }
  }
}
