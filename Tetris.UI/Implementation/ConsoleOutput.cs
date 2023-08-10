using Tetris.UI.Interfaces;

namespace Tetris.UI.Implementation;
internal class ConsoleOutput : IOutputSource
{
  public void Clear() => Console.Clear();

  public void SetColor(ConsoleColor color)
  {
    Console.ForegroundColor = color;
  }

  public void Update(string message)
  {
    Console.Clear();
    Write(message);
  }

  public void Write(string message)
  {
    Console.WriteLine(message);
  }

  public void WriteSymbol(char symbol)
  {
    Console.Write(symbol);
  }
}
