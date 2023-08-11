namespace Tetris.UI.Interfaces;
internal interface IOutputSource
{
  void Write(string message);
  void Update(string message);
  void Clear();

  void WriteSymbol(char symbol);

  void SetColor(ConsoleColor color);
}
