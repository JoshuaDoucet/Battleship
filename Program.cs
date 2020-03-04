//Programmer: Joshua Doucet
//Date: February 17 2020
//Project: Battleship
//Description: A 1-player version of the boardgame classic "Battleship"
//       The game will prompt the user for the gameboard configurations such as
//       the number of ships and board size. Once a configuration is chosen, the game
//       autogenerates a gameboard with the ships randomly placed.
//       The game prompts the user when a ship is sunk, and the game ends when all ships are sunk.
//       A "hack mode" is included that displays the location of ships on the board. If you wish to use
//       this feature just enter "Y" when asked if you are a hacker.
//       The game will continue to start new games until you tell the game you do not want to play again.
//
//Filename: Program.cs - this is the entry point for code execution.

using System;

namespace JDoucetBattleship
{
   class Program
   {
      static void Main(string[] args)
      {
         Game game = new Game();
      }
   }
}
