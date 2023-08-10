namespace Tetris.UI.Interfaces;
internal interface IInputSource
{
  ConsoleKey ReadKey();
  string ReadLine();
}
