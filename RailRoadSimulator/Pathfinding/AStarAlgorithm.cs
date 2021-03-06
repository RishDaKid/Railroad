﻿using RailRoadSimulator.Factories.LayoutFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadSimulator.Pathfinding
{

	public class PathFinding
	{
		//final list to go
		List<Tile> finalList = new List<Tile>();

		/// <summary>
		/// This finds the path
		/// </summary>
		/// <param name="map">the layout of the map</param>
		/// <param name="startLoc">startlocation</param>
		/// <param name="endLoc">endlocation</param>
		public void findTiles(List<string> map, char startLoc, char endLoc)
		{
			//This gets the start coordinates of the train
			var start = new Tile();
			start.Y = map.FindIndex(x => x.Contains(startLoc));
			start.X = map[start.Y].IndexOf(startLoc);

			//This gets the end coordinates of the train
			var finish = new Tile();
			finish.Y = map.FindIndex(x => x.Contains(endLoc));
			finish.X = map[finish.Y].IndexOf(endLoc);

			start.SetDistance(finish.X, finish.Y);

			var activeTiles = new List<Tile>();
			activeTiles.Add(start);
			var visitedTiles = new List<Tile>();

			while (activeTiles.Any())
			{
				var checkTile = activeTiles.OrderBy(x => x.CostDistance).First();

				if (checkTile.X == finish.X && checkTile.Y == finish.Y)
				{
					//We found the destination and we can be sure (Because the the OrderBy above)
					//That it's the most low cost option. 
					var tile = checkTile;
					Console.WriteLine("Retracing steps backwards...");
					while (true)
					{
						Console.WriteLine($"{tile.X} : {tile.Y}");
						if (map[tile.Y][tile.X] != startLoc || map[tile.Y][tile.X] != ' ' || map[tile.Y][tile.X] != endLoc)
						{
							//convert the tiles with their coordinates to the right path
							var newMapRow = map[tile.Y].ToCharArray();
							newMapRow[tile.X] = '*';
							map[tile.Y] = new string(newMapRow);
						}
						finalList.Add(tile);
						tile = tile.Parent;
						if (tile == null)
						{
							//reverse the final list to ensure it is from the start to the end 
							finalList.Reverse();
							Console.WriteLine("Map looks like :");
							map.ForEach(x => Console.WriteLine(x));
							Console.WriteLine("Done!");
							return;
						}
					}
				}

				visitedTiles.Add(checkTile);
				activeTiles.Remove(checkTile);

				var walkableTiles = GetWalkableTiles(map, checkTile, finish, endLoc);

				foreach (var walkableTile in walkableTiles)
				{
					//We have already visited this tile so we don't need to do so again!
					if (visitedTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
						continue;

					//It's already in the active list, but that's OK, maybe this new tile has a better value (e.g. We might zigzag earlier but this is now straighter). 
					if (activeTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
					{
						var existingTile = activeTiles.First(x => x.X == walkableTile.X && x.Y == walkableTile.Y);
						if (existingTile.CostDistance > checkTile.CostDistance)
						{
							activeTiles.Remove(existingTile);
							activeTiles.Add(walkableTile);
						}
					}
					else
					{
						//We've never seen this tile before so add it to the list. 
						activeTiles.Add(walkableTile);
					}
				}
			}

			Console.WriteLine("No Path Found!");


		}
		/// <summary>
		/// Gets possible tiles to go to
		/// </summary>
		/// <param name="map">the layout</param>
		/// <param name="currentTile">tile where currenly is</param>
		/// <param name="targetTile">target tile to go to</param>
		/// <param name="endLoc">end location</param>
		/// <returns>next possible tiles in a list</returns>
		private static List<Tile> GetWalkableTiles(List<string> map, Tile currentTile, Tile targetTile, char endLoc)
		{
			var possibleTiles = new List<Tile>()
			{
				new Tile { X = currentTile.X, Y = currentTile.Y - 1, Parent = currentTile, Cost = currentTile.Cost + 1 },
				new Tile { X = currentTile.X, Y = currentTile.Y + 1, Parent = currentTile, Cost = currentTile.Cost + 1},
				new Tile { X = currentTile.X - 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
				new Tile { X = currentTile.X + 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
			};

			possibleTiles.ForEach(tile => tile.SetDistance(targetTile.X, targetTile.Y));

			var maxX = map.First().Length - 1;
			var maxY = map.Count - 1;
			//This are the ways a train can go
			return possibleTiles
					.Where(tile => tile.X >= 0 && tile.X <= maxX)
					.Where(tile => tile.Y >= 0 && tile.Y <= maxY)
					.Where(tile => map[tile.Y][tile.X] != ' ')
					.ToList();
		}
	}
	}


