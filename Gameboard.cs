//Filename: Gameboard.cs - this is the class file that describes a battleship gameboard.
//             this class has methods for displaying and clearing a Gameboard.
//             the board is altered through its properties

using System;
using System.Collections.Generic;
using System.Text;

namespace JDoucetBattleship
{
   //Possible non-null values in the gameboard grid
   public enum BoardSymb { Hit = 'O', Miss = 'X', Ship = 'S' };
   class Gameboard
   {
      int rows;
      int cols;
      //2D gameboard grid
      char[,] grid;
      //if hack mode is on, then (S's) will be visible on the gameboard instead of spaces
      bool hackMode;

      public Gameboard(int rows, int cols, bool hack)
      {
         Rows = rows;
         Cols = cols;
         Grid = new char[Rows, Cols];
         hackMode = hack;
      }

      public int Rows { get => rows; set => rows = value; }
      public int Cols { get => cols; set => cols = value; }
      public char[,] Grid { get => grid; set => grid = value; }
      public bool HackMode { get => hackMode; set => hackMode = value; }

      //Clears the board by placing null values in all the entries
      public void ClearBoard()
      {
         for (int row = 0; row < Rows; row++)
            for (int col = 0; col < Cols; col++)
               Grid[row, col] = '\0';
      }

      //Diaplsys the gameboard with numberic col headers, and alphabetic row labels.
      //The board is also displayed woth ASCII grid borders, and the values stored in Grid
      public void DisplayBoard()
      {
         int gridColLabelNum;
         int gridRowLabelNum = 0;
         //Board sizes needed to account for ASCII board art as well as coll and row labels
         int boardWidth = Cols * 4 + 2;
         int boardHeight = Rows * 2 + 2;

         //Add a blank line before displaying board
         Console.WriteLine();

         //For each console row
         for (int i = 0; i < boardHeight; i++)
         {
            gridColLabelNum = 0;
            //For each character in each console row
            for (int j = 0; j < boardWidth; j++)
            {
               //Setup the col numbers at the top of the board
               //Place a number after every 3 spaces on the first line, 
               //or after every 2 spaces once numbers become 2 digits
               if (i == 0 && j % 4 != 3 && j < 41)
                  Console.Write(" ");
               //After j = 40, header numbers become 2-digits and require 1 less space between numbers
               else if (i == 0 && j >= 41 && (j % 4 == 1 || j % 4 == 2))
                  Console.Write(" ");
               //If a space has not been printed, then print col num header
               else if (i == 0)
               {
                  Console.Write(gridColLabelNum + 1);
                  //Increment the col header label if it has not reached the final col index
                  if (gridColLabelNum + 1 < Cols)
                     gridColLabelNum++;
                  //if col num >= 10 increment j an extra time since header nums are now double digit
                  if (gridColLabelNum >= 10)
                     j++;
               }

               //if the board row is odd, then starting at board collumn 1, 
               //print a '+' every 4 collumns, otherwise print a "-" on odd board rows
               if (i % 2 == 1 && j == 0)
               {
                  Console.Write(" ");
               }
               else if (i % 2 == 1 && (j == 0 || j % 4 == 1))
               {
                  Console.Write("+");
               }
               else if (i % 2 == 1)
               {
                  Console.Write("-");
               }

               //if board row is even and NOT the 0th row
               //and if board col is the 0th col, then print the row letter
               if (i % 2 == 0 && i != 0 && j == 0)
               {
                  //Add 65 to rowHeaderNum to get integer representation of the alphabet character
                  Console.Write((char)(gridRowLabelNum + 65));
                  //Increment the row letter label if it has not reached the final row index
                  if(gridRowLabelNum < Rows)
                     gridRowLabelNum++;
               }
               //On even numbered board rows print a "|" every 4 cols after the row label
               else if (i % 2 == 0 && i != 0 && j % 4 == 1)
               {
                  Console.Write('|');
               }
               //On even numbered board rows other than the header row of the board,
               //print the gameboard grid symbol (Hit, Miss, Ship, ' ' ) stored in Grid every 4 cols 
               //starting at col index 3
               else if (i % 2 == 0 && j % 4 == 3 && i != 0) 
               {
                  char gridVal = Grid[gridRowLabelNum -1, gridColLabelNum];
                  //If grid value is null print a space or
                  //if hackMode disabled, print a space instead of Ship character
                  if (gridVal == '\0' || (gridVal == (char)BoardSymb.Ship && !HackMode))
                     Console.Write(' ');
                  else
                     Console.Write(gridVal);
                  gridColLabelNum++;
               }
               //If the row is even and nothing else has been printed, then print a space
               else if (i % 2 == 0 && j > 0 && i > 0)
               {
                  Console.Write(' ');
               }
            }
            //jump down to next row
            Console.Write("\n");
         }
            
      }
   }
}
