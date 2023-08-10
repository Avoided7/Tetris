using Tetris.UI.Interfaces;

namespace Tetris.UI.Implementation;
internal class ConsoleInput : IInputSource
{
  public ConsoleKey ReadKey()
  {
    return Console.ReadKey().Key;
  }

  public string ReadLine()
  {
    return Console.ReadLine()!;
  }
}
