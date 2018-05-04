# Maze Solver

Author: Steve Lent

# About
This is a project to create an ASP.NET Web API project. Given a text maze in a POST request, it will solve it, and return a JSON object with the solved maze, in the minimal amount of steps and time, and the number of moves necessary to make solve it. It runs on port 8080, with the endpoint 'solveMaze'.
It has a test HTML with some HTML and Javascript which contains the original three mazes plus some mazes that cannot be solved, for test purposes.

In addition, the user can choose to close the search bar to preserve space in a device with limited screen real estate.

# Setup
Download or clone the repo [here](https://github.com/velcromagnon/MazeSolver). Build the project in Visual Studio 2017, and run it. It will host on port 8080 and start the client with the index.html in the project.
Or you can run it directly from [here](some link)

# Usage
It has a dropdown of the six included mazes, and when the maze is selected, it will issue a call to the Web API to solve the maze and return the result, which is displayed below.