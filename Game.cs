//Filename: Game.cs - this is the class file that describes a battleship game.
//             this is where all of the messy battleship game logic unfolds
//             A game can be started/setup, a game can get user input to fire on ships,
//             a game can update the board, and a game eventually does end.

using System;
using System.Collections.Generic;
using System.Text;

namespace JDoucetBattleship
{
   class Game
   {
      //Constants for different ship lengths found in a battleship game
      enum ShipSizes
      {
         Carrier = 5,
         Battleship = 4,
         Submarine = 3,
         Destroyer = 2
      };

      //Used for random ship positions on the gameboard
      private Random random;

      //Used as the game playing field where ships will be located.
      private Gameboard board;

      //total number of ships in the game
      private int numShips;

      //An array of all ships that will be on the board
      private Ship[] ships;

      //Number of ships that have sunk
      private int sunkShips;

      public int NumShips { get => numShips; set => numShips = value; }
      public int SunkShips { get => sunkShips; set => sunkShips = value; }
      internal Ship[] Ships { get => ships; set => ships = value; }
      internal Gameboard Board { get => board; set => board = value; }
      public Random Rand { get => random; set => random = value; }

      //Game constructor
      public Game()
      {
         //Initialize random object for game
         Rand = new Random();

         //Start game loop
         Start();
      }

      //Method that checks if any ships have recently been sunk
      //If a ship has been sunk, update game states and print sunk message to console
      private void CheckSunk()
      {
         //Used to traverse the grid positions that a ship occupys
         int traverseX, traverseY, traverseCount;
         //Number of times a ship has been hit (in different grid positions)
         int hitCount;

         //Check all ships to see if any new ships have sunk
         for (int shipNum = 0; shipNum < NumShips; shipNum++)
         {
            //If the current ship has not sunk yet, check if it just sunk
            if (!Ships[shipNum].Sunk)
            {
               //Starting position to check for sunk ship
               traverseX = Ships[shipNum].BowPosX;
               traverseY = Ships[shipNum].BowPosY;
               //Number of ship positions traversed
               traverseCount = 0;
               //Number of ship positions hit
               hitCount = 0;

               //Check each position of a ship to determine how many positions have been hit
               while (traverseCount < Ships[shipNum].Length)
               {
                  //Increment hitCount if the traverse position on the board is a Hit symbol
                  if (Board.Grid[traverseY, traverseX] == (char)BoardSymb.Hit)
                     hitCount++;

                  //if vertical go to next ship position on grid (below current traverse position)
                  if (Ships[shipNum].Vertical)
                     traverseY++;
                  //Otherwise the ship is horizontal 
                  //go to next ship position on grid (right of current traverse position)
                  else
                     traverseX++;

                  traverseCount++;
               }

               //If a ship has sunk, then update Ship and Game fields, and print console message
               if (hitCount == Ships[shipNum].Length)
               {
                  Ships[shipNum].Sunk = true;
                  SunkShips++;
                  PrintSunkMsg(Ships[shipNum].Length);
               }
            }
         }
      }

      //Prints a message that describe a ship that was sunk based on its length
      private void PrintSunkMsg(int shipLength)
      {
         switch (shipLength)
         {
            case (int)ShipSizes.Battleship:
               Console.WriteLine("You sunk a Battleship!");
               break;
            case (int)ShipSizes.Carrier:
               Console.WriteLine("You sunk a Carrier!");
               break;
            case (int)ShipSizes.Submarine:
               Console.WriteLine("You sunk a Submarine.");
               break;
            case (int)ShipSizes.Destroyer:
               Console.WriteLine("You sunk a Destroyer.");
               break;
            default:
               Console.WriteLine("Error. Unknown ship has been sunk...");
               break;
         }
      }

      //Populates the Ships array with the correct size Ships and then places the ships on the gameboard.
      //This methos returns false if 1 or more ships could not be placed on the board.
      private bool InitShips()
      {
         //Populate the Ships array with the correct length ships
         Ships = new Ship[NumShips];
         PopulateShipsLength();
         //Find a place for each ships on the gameboard
         return PlaceShips();
      }

      //Method that takes a Ship and finds a random place for the ship on the Gameboard
      //If the ship cannot be placed, this method returns false.
      private bool FindSpotForShip(Ship ship)
      {
         //Has a position for the ship been found?
         bool posFound = false;
         int attempts = 0;
         //try to find a home for the ship on the gameboard
         //Reach a fail state if attempts reaches 1000
         while (attempts <= 1000 && !posFound)
         {
            //Give ship a random verticality
            SetRandShipVerticality(ship);
            //Give ship a random bow pos that lets it fit in the board
            SetRandBowPos(ship);
            //Checks if the ship will fit on the board given its new verticaliy and bow position
            posFound = ShipPosAvailable(ship);
            //The random ship placement does not fit. increment attempts
            attempts++;
         }
         return posFound;
      }

      //Givin a firing position (row, col), check to see if it hits any Ships on the Board.
      //Update the board with the corresponding ressult, (Hit or Miss symbol)
      //If the firePosition hits a ship, update the Board with a Hit synbol in that pos
      //Otherwise update the board with an a Miss symbol
      private void FireWeapons(int[] firePosition)
      {
         char boardValue = Board.Grid[firePosition[0], firePosition[1]];
         if (boardValue == (char)BoardSymb.Ship)
         {
            Board.Grid[firePosition[0], firePosition[1]] = (char)BoardSymb.Hit;
            Console.WriteLine("Hit!");
         } else if (boardValue == '\0')
         {
            Board.Grid[firePosition[0], firePosition[1]] = (char)BoardSymb.Miss;
            Console.WriteLine("Miss.");
         }
      }
      
      //Method that gets board coordinates from the user and converts them into indecies
      //that correspond to an entry in Board.Grid
      //The 0th element in the returned array is the row index
      //The 1st element in the returned array is the col index
      private int[] GetFiringCoordinates()
      {
         const int INVALID_INPUT = -343;
         //Complete user input string
         string input;
         //used to parse string input into integer column index for Board.Grid
         string colStr;
         //Indecies that will be returned that correspond to the Board.Grid
         int row = INVALID_INPUT, col = INVALID_INPUT;
         //Flag for valid user input
         bool validInput = false;

         //Get input, if input is invalid, loop until it is valid
         Console.WriteLine("\nInput a grid target. (Example C5 or A10)");
         while (!validInput)
         {
            //Assume valid input, but flag it if checks out as invalid input
            validInput = true;
            input = Console.ReadLine();

            //Chheck for valid input length
            if (input.Length > 1 && input.Length < 4)
            {
               //Convert letter to a board row index (A starts at 0) Ex: A is 65 ASCII and is also Board.Grid[0], 
               //If 'A' is zeroth input char, then row = 0 
               row = ((int)Char.ToUpper(input[0])) - 65;

               //Get all of the user input except for the zeroth character 
               //(So, for a proper input instance grab the grid number and skip the grid letter)
               colStr = input.Substring(1, input.Length - 1);
               //Ensure length is greater than 1 to avoid FormatException from invalid input

               //Convert the user input column string into an integer
               //If conversion is unsuccessful flag for invalid data
               if (!int.TryParse(colStr, out col))
               {
                  validInput = false;
                  Console.WriteLine("Invalid Input Format. Try Again.");
               }
               //Conversion from string to int for col index was successful
               else
               {
                  //Subtract 1 from col, Displayed board starts at 1, but Board.Grid starts at 0, 
                  col -= 1;
                  //The input must correspond to rows in the Board.Grid
                  if (row < 0 || col < 0 || row >= Board.Rows || col >= Board.Cols)
                  {
                     validInput = false;
                     Console.WriteLine("Input Out of Bounds. Try Again.");
                  }
               }
            }
            //Invalid input length
            else
            {
               validInput = false;
               Console.WriteLine("Invalid Input Length. Try Again.");
            }
         }

         //Return the row and col of Board.Grid that corresponds to the user input.
         return new int[] { row, col };
      }

      //Given a Ship object, place it on the Board by updating the board with the Ship Symbol
      private void PlaceShip(Ship ship)
      {
         /*
          * If a ship is Vertical then the ship will be placed on the board start at 
          * the bowPos and the rest of the ship will be placed on the grid BELLOW of that position
          * 
          * If a ship is Not-Vertical (Horizontal) then the ship will be placed on the board start at 
          * the bowPos and the rest of the ship will be placed on the grid to the RIGHT of that position
          * 
          */

         if (ship.Vertical)
            for (int i = 0; i < ship.Length; i++)
               Board.Grid[ship.BowPosY + i, ship.BowPosX] = (char)BoardSymb.Ship;
         else
            for (int i = 0; i < ship.Length; i++)
               Board.Grid[ship.BowPosY, ship.BowPosX + i] = (char)BoardSymb.Ship;
      }

      //Place ALL ships in Ships on the Board
      //finds a position on the board that does not interfere with other ships on the board
      //if any ship cannot find a valid position, then the method returns false.
      private bool PlaceShips()
      {
         bool success = true;
         bool available;
         //find a bow position for each ship in Ships
         for (int shipNum = 0; shipNum < NumShips; shipNum++)
         {
            available = FindSpotForShip(Ships[shipNum]);
            if (available)
               PlaceShip(Ships[shipNum]);
            else
               success = false;
         }
         return success;
      }


      //Method that prompts a user for board coordinates, fires on those coordinates, updates the board,
      //and checks if any ships have sunk
      private void PlayTurn()
      {
         //Get the users guess at where a ship is located on the board
         int[] firePosition = GetFiringCoordinates();
         //Fire weapons, and update the Board with a hit or miss symbol
         FireWeapons(firePosition);
         //Determine is the weapon firing has sunk any ships
         CheckSunk();
         //TODO
      }

      //Populates the length field of all ships in Ships.
      //The first ship is a Carrier, 2nd is a Battleship, 3rd & 4th are Submarines
      //5th and 6th are destroyers, and ALL OTHER ships are Submarines
      private void PopulateShipsLength()
      {
         for (int shipNum = 0; shipNum < NumShips; shipNum++)
         {
            Ships[shipNum] = new Ship();
            switch (shipNum)
            {
               //ship 0 is a Carrier
               case 0:
                  Ships[shipNum].Length = (int)ShipSizes.Carrier;
                  break;
               //ship 1 is a Battleship
               case 1:
                  Ships[shipNum].Length = (int)ShipSizes.Battleship;
                  break;
               //ship 2 and 3 and 6+ are Submarines
               case 2:
               case 3:
               default:
                  Ships[shipNum].Length = (int)ShipSizes.Submarine;
                  break;
               //Ships 4 and 5 are Destroyers
               case 4:
               case 5:
                  Ships[shipNum].Length = (int)ShipSizes.Destroyer;
                  break;
            }
         }
      }

      //Give a Ship, set a random bow pos that lets it fit in the board boundaries
      private void SetRandBowPos(Ship ship)
      {
         int randXPos;
         int randYPos;

         /*
          * The following if/else block ensure that the entire ship can fit within the gameboard,
          * but does NOT check if the ship interferes with other ships
          */

         if (ship.Vertical)
         {
            randXPos = Rand.Next(0, board.Cols);
            randYPos = Rand.Next(0, board.Rows - ship.Length);
         }
         else //if ship is horizontal...
         {
            randXPos = Rand.Next(0, board.Cols - ship.Length);
            randYPos = Rand.Next(0, board.Rows);
         }

         ship.BowPosX = randXPos;
         ship.BowPosY = randYPos;
      }

      //Give a Ship a random verticality
      private void SetRandShipVerticality(Ship ship)
      {
            ship.Vertical = (Rand.Next(0, 2) == 1) ? true: false;
      }

      //Given a ship determine if its bow position, length, and verticality allow it to fit
      //on the board without coliding wiht other ships. This method does NOT check if the Ships
      //fits within the Boards borders
      //Returns true if the Ship's current position is available on the board.
      private bool ShipPosAvailable(Ship ship)
      {
         //Used to store a value from the board grid to determine if a ship is placed there already
         char checkChar;

         //Determine if the ship collides with other ships already on the board
         //compare the ship position to the values in the ship board grid

         //If vertical check the grid values bellow the bowPos for the length of the ship
         if (ship.Vertical)
            for (int i = 0; i < ship.Length; i++)
            {
               checkChar = Board.Grid[ship.BowPosY + i, ship.BowPosX];
               if (checkChar == (char)BoardSymb.Ship)
                  return false;
            }
         //If horizontal check the grid values right of the bowPos for the length of the ship
         else
            for (int i = 0; i < ship.Length; i++)
            {
               checkChar = Board.Grid[ship.BowPosY, ship.BowPosX + i];
               if (checkChar == (char)BoardSymb.Ship)
                  return false;
            }

         return true;
      }

      //Sets up the game by prompting a user for game configuration details,
      //and then initializing game objects. Once finished the Board will be displayed.
      private void GameSetup()
      {
         //Variable for custom games
         int numShips, rows, cols;
         bool hacker = true;
         char inputCh;

         //Flag that determines if a custom game successfully placed all ships on board
         bool setupSuccess = false;

         //No ships are sunk at the beginning of a game.
         SunkShips = 0;

         //Promt user for game mode
         Console.WriteLine("Choose a game mode number.\n"
                         + "  (1) Classic - 6 ships on a 10 x 10 grid.\n"
                         + "  (2) Custom - Change the number of ships and board size.");

         //if invalid input, try until input is valid
         int menuInput = 0;
         while (menuInput < 1 || menuInput > 2)
         {
            try { menuInput = Convert.ToInt32(Console.ReadLine()); }
            catch { }
            if (menuInput < 1 || menuInput > 2)
               Console.WriteLine("Invalid choice. Try again.");
         }

         //Find out if the user is a hacker
         Console.WriteLine("Are you a hacker? (Y/N)");
         inputCh = Char.ToUpper((char)Console.ReadLine()[0]);
         if (inputCh == 'Y') { hacker = true; } else { hacker = false; }

         //If user chooses Classic game
         if (menuInput == 1)
         {
            //Initialize ships and add them to gameboard
            NumShips = 6;

            //Initialize gameboard
            Board = new Gameboard(10, 10, hacker);

            //Get the ships ready for the game
            InitShips();
         }
         //if user chooses Custom game
         else
         {
            //The setup success ensures the user does not choose a num of ships that cannot fit on the board
            while (!setupSuccess)
            {
               //Get custom game details from user and check for valid input until valid input is entered
               numShips = 0;
               while (numShips < 1 || numShips > 150)
               {
                  Console.WriteLine("Number of ships?");
                  try { numShips = Convert.ToInt32(Console.ReadLine()); }
                  catch { }
                  if (numShips < 1)
                     Console.WriteLine("Invalid number of ships. Try agian.");
               }

               rows = 0;
               while (rows < 5 || rows > 26)
               {
                  Console.WriteLine("Number of board rows? (5 - 26)");
                  try { rows = Convert.ToInt32(Console.ReadLine()); }
                  catch { }
                  if (rows < 5 || rows > 26)
                     Console.WriteLine("Invalid number of rows. Try agian.");
               }

               cols = 0;
               while (cols < 5 || cols > 26)
               {
                  Console.WriteLine("Number of board cols? (5 - 26)");
                  try { cols = Convert.ToInt32(Console.ReadLine()); }
                  catch { }
                  if (cols < 5 || cols > 26)
                     Console.WriteLine("Invalid number of cols. Try agian.");
               }

               //Assign the custom number of ships
               NumShips = numShips;

               //Initialize custom gameboard
               Board = new Gameboard(rows, cols, hacker);

               //Get the ships ready for the game
               setupSuccess = InitShips();

               //If the user entered configuration cannot be setup, pormpt the user and try again
               if (!setupSuccess)
                  Console.WriteLine("All ships could not be placed. Try another game board configuration.");
            }
         }

         //Show the user the pretty ASCII battleship board
         Board.DisplayBoard();
      }

      //Method that starts the battleship game!!!
      //The main game loops exist here in this method.
      public void Start()
      {
         Console.WriteLine("Welcome to Battleship!");
         Console.WriteLine("------------------------");

         //The game should be played at least once...
         char playAgain = 'Y';

         //Pick the game mode and setup game states
         GameSetup();

         //While the user wants to play another game
         while (playAgain == 'Y')
         {
            //While all ships are not sunk keep making the user choose grid positions to attack
            while (NumShips > SunkShips)
            {
               PlayTurn();
               Board.DisplayBoard();
            }

            //Prompt the user that the game is over, and ask if they want to play again
            Console.WriteLine("All ships have been sunk! Play Again? (Y/N)");
            playAgain = Char.ToUpper((char)Console.ReadLine()[0]);
            if (playAgain == 'Y')
               GameSetup();
         }
      }
   }
}
