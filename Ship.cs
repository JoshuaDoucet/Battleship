//Filename: Ship.cs - this is the class file that describes a battleship ship.
//             this class has no methods (just properties) and acts as state holder
//             for Ship information in a game of battleship. 

using System;
using System.Collections.Generic;
using System.Text;

namespace JDoucetBattleship
{
   class Ship
   {
      //Grid spaces occupied by a ship
      private int length;

      //The head of the ship
      private int bowPosX; //coll position
      private int bowPosY; //row position
      //If the ship is horizontally or vertically placed on a 2D grid
      private bool vertical;
      //Has the ship sunk or not
      private bool sunk;

      public Ship()
      {
         Length = 2;
         BowPosX = 0;
         BowPosY = 0;
         Vertical = false;
         Sunk = false;
      }

      public Ship(int length, int bowPosX, int bowPosY, bool vetical)
      {
         Length = length;
         BowPosX = bowPosX;
         BowPosY = bowPosY;
         Vertical = vertical;
      }

      public int Length { get => length; set => length = value; }
      public int BowPosX { get => bowPosX; set => bowPosX = value; }
      public int BowPosY { get => bowPosY; set => bowPosY = value; }
      public bool Vertical { get => vertical; set => vertical = value; }
      public bool Sunk { get => sunk; set => sunk = value; }
   }
}
