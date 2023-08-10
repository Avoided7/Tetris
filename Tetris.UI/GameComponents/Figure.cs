using Tetris.UI.Enums;
using Tetris.UI.Utilities;

namespace Tetris.UI.GameComponents;
internal struct Figure
{
  public int Height { get; set; }
  public int Width { get; set; }
  public int Rotation { get; private set; }
  public int[,] Volume { get; set; } = null!;

  public Figure(int height, int width, FigureType figureType = FigureType.Block) : this (figureType)
  {
    Height = height;
    Width = width;
  }
  public Figure(FigureType figureType = FigureType.Block)
  {
    SetVolume(figureType);
  }

  public void Rotate()
  {
    Rotation = (Rotation + 90) % 360;
  }

  private void SetVolume(FigureType figureType)
  {
    switch (figureType)
    {
      case FigureType.Block:
        Volume = Constants.Block;
        break;
      case FigureType.Line:
        Volume = Constants.Line;
        break;
      case FigureType.ObjectL:
        Volume = Constants.ObjectL;
        break;
      default:
        throw new ArgumentException($"Incorrect argument. {nameof(figureType)}");
    }
  }
}
