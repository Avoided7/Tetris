namespace Tetris.UI.Utilities;
internal static class Constants
{
  public const int FIGURE_TYPES_COUNT = 3;

  public static readonly int[,] Block =
  {
    { 1, 1 },
    { 1, 1 },
  };

  public static readonly int[,] Line =
  {
    { 1, 1, 1, 1 }
  };

  public static readonly int[,] ObjectL =
  {
    { 0, 1 },
    { 0, 1 },
    { 1, 1 },
  };

  public const char WALL_SYMBOL = '#';
  public const char BLOCK_SYMBOL = '*';
  public const char EMPTY_SYMBOL = '.';

  public const ConsoleColor BLOCK_COLOR = ConsoleColor.Cyan;
  public const ConsoleColor BASE_COLOR = ConsoleColor.White;
}
