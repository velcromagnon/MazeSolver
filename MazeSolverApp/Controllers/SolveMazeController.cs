using MazeSolverApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;

namespace MazeSolverApp.Controllers
{
    public class SolveMazeController : ApiController
    {
        public IHttpActionResult Post([FromBody] SolveMaze maze)
        {
            // This is a maze with newlines. Split string on newlines
            string[] lines = maze.map.Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );
            // Now we have an array that we can solve!
            string errorMessage = "";
            string solvedMaze = "";
            int numberOfSteps = -1;
            bool result = SolveMaze(lines, ref errorMessage, ref solvedMaze, ref numberOfSteps);
            if (result == false)
            {
                maze.count = -1;
                maze.map = "No solution";
            }
            else
            {
                maze.count = numberOfSteps;
                maze.map = solvedMaze;
            }

            return Ok(maze);
        }
        public class Point
        {
            public int X { get; set; }
            public int Y { get; set; }
            public Point(int y, int x)
            {
                Y = y;
                X = x;
            }
        }

        private bool SolveMaze(String[] maze, ref string errorMessage, ref string solvedMaze, ref int numberOfSteps)
        {
            numberOfSteps = -1;
            solvedMaze = "";

            if (maze.Length == 0)
            { 
                errorMessage = "Empty maze";
                return false;
            }
            // Create a queue. This will allow an efficient way of traversing the maze breadth-first. The technique is called memoization.
            Queue<Point> q = new Queue<Point>();
            int startX = 0, startY = 0;
            int endX = -1, endY = -1;

            int xSize = maze[0].Length;
            int ySize = maze.Length;
            int[,] bestPath = new int[ySize, xSize];

            // Set up the starting point and give it a value of 0.
            bool foundStartingPoint = false;
            for (int y = 0; y < ySize; y++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    if (maze[y][x] == 'A')
                    {
                        foundStartingPoint = true;
                        startX = x;
                        startY = y;

                        bestPath[y, x] = 0; // 0 moves.
                    }
                    else
                        bestPath[y, x] = -1; // Unknown
                }
            }
            if (!foundStartingPoint)
            {
                errorMessage = "No starting point";
                return false;
            }
            // Add the starting pair to a stack.
            q.Enqueue(new Point(startX, startY));

            bool solved = false;

            // Keep enqueuing new points until it's either solved or the queue is empty.

            while (q.Count > 0 && !solved)
            {
                Point p = q.Dequeue();
                // Found the destination?
                if (maze[p.Y][p.X] == 'B')
                {
                    endY = p.Y;
                    endX = p.X;
                    solved = true;
                }
                else
                {
                    if (p.X > 0) // Try left.
                    {
                        if (maze[p.Y][p.X - 1] != '#' &&
                            bestPath[p.Y, p.X - 1] == -1)
                        {
                            bestPath[p.Y, p.X - 1] = bestPath[p.Y, p.X] + 1;
                            q.Enqueue(new Point(p.Y, p.X - 1));
                        }
                    }
                    if (p.X < xSize - 1) // Try right.
                    {
                        if (maze[p.Y][p.X + 1] != '#' &&
                            bestPath[p.Y, p.X + 1] == -1)
                        {
                            bestPath[p.Y, p.X + 1] = bestPath[p.Y, p.X] + 1;
                            q.Enqueue(new Point(p.Y, p.X + 1));
                        }
                    }
                    if (p.Y > 0) // Try up.
                    {
                        if (maze[p.Y - 1][p.X] != '#' &&
                            bestPath[p.Y - 1, p.X] == -1)
                        {
                            bestPath[p.Y - 1, p.X] = bestPath[p.Y, p.X] + 1;
                            q.Enqueue(new Point(p.Y - 1, p.X));
                        }
                    }
                    if (p.Y < ySize - 1) // Try down.
                    {
                        if (maze[p.Y + 1][p.X] != '#' &&
                            bestPath[p.Y + 1, p.X] == -1)
                        {
                            bestPath[p.Y + 1, p.X] = bestPath[p.Y, p.X] + 1;
                            q.Enqueue(new Point(p.Y + 1, p.X));
                        }
                    }
                }
            }
            // Now go through and backtrack back to the start, only looking for nodes that are one less.
            StringBuilder[] sb = new StringBuilder[ySize];
            for (int i = 0; i < ySize; i++)
            {
                sb[i] = new StringBuilder(maze[i]);
            }
            if (endX == -1 || endY == -1)
            {
                errorMessage = "Could not find a path through the maze";
                return false;
            }
            else
            {
                // Backtrack
                int curX = endX, curY = endY, count = bestPath[endY, endX];
                while (count > 0) // Skip the last one, don't want to overwrite it.
                {
                    // Go up
                    if (curY > 0 && bestPath[curY - 1, curX] == bestPath[curY, curX] - 1)
                    {
                        curY--;
                        if (sb[curY][curX] == '.')
                           sb[curY][curX] = '@';
                    }
                    // Go down
                    if (curY < ySize - 1 && bestPath[curY + 1, curX] == bestPath[curY, curX] - 1)
                    {
                        curY++;
                        if (sb[curY][curX] == '.')
                            sb[curY][curX] = '@';
                    }
                    // Go left
                    if (curX > 0 && bestPath[curY, curX - 1] == bestPath[curY, curX] - 1)
                    {
                        curX--;
                        if (sb[curY][curX] == '.')
                            sb[curY][curX] = '@';
                    }
                    // Go right
                    if (curX < xSize - 1 && bestPath[curY, curX + 1] == bestPath[curY, curX] - 1)
                    {
                        curX++;
                        if (sb[curY][curX] == '.')
                            sb[curY][curX] = '@';
                    }
                    count = bestPath[curY, curX];
                }
                DateTime dtEnd = DateTime.Now;

                //                var diff = dtEnd - dtStart;
                solvedMaze = "";
                foreach (StringBuilder s in sb) // Now append strings with newlines 
                {
                    // Console.WriteLine(s);
                    solvedMaze += (s + System.Environment.NewLine);
                }
                numberOfSteps = bestPath[endY, endX];
            }
            return true;
        }
    }
}
