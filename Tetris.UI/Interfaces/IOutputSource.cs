using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.UI.Interfaces;
internal interface IOutputSource
{
  void Write(string message);
  void Update(string message);
  void Clear();

  void WriteSymbol(char symbol);

  void SetColor(ConsoleColor color);
}
