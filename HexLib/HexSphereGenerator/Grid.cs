using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HexLib.HexSphereGenerator
{
    public class Grid
    {
        public int Size { get; }
        public List<Tile> Tiles { get; } = new List<Tile>();
        public List<Corner> Corners { get; } = new List<Corner>();
        public List<Edge> Edges { get; } = new List<Edge>();

        public Grid(int size)
        {
            Size = size;

            for (int i = 0; i < GetTileCount(size); i++)
                Tiles.Add(new Tile(i, i < 12 ? 5 : 6));

            for (int i = 0; i < GetCornerCount(size); i++)
                Corners.Add(new Corner(i));

            for (int i = 0; i < GetEdgeCount(size); i++)
                Edges.Add(new Edge(i));
        }

        private static int GetTileCount(int size)
        {
            return 10 * (int)Math.Pow(3, size) + 2;
        }

        private static int GetCornerCount(int size)
        {
            return 20 * (int)Math.Pow(3, size);
        }

        private static int GetEdgeCount(int size)
        {
            return 30 * (int)Math.Pow(3, size);
        }

        public static Grid CreateSizeNGrid(int size)
        {
            Debug.Assert(size >= 0);

            if (size == 0)
            {
                return CreateSize0Grid();
            }
            else
            {
                return CreateSizeNGrid(size - 1).CreateSubdividedGrid();
            }
        }

        public static Grid CreateSize0Grid()
        {
            Grid grid = new Grid(0);
            double x = -0.525731112119133606;
            double z = -0.850650808352039932;

            Vector3[] icos_tiles = new[] {
                new Vector3(-x, 0, z), new Vector3(x, 0, z), new Vector3(-x, 0, -z), new Vector3(x, 0, -z),
                new Vector3(0, z, x), new Vector3(0, z, -x), new Vector3(0, -z, x), new Vector3(0, -z, -x),
                new Vector3(z, x, 0), new Vector3(-z, x, 0), new Vector3(z, -x, 0), new Vector3(-z, -x, 0)
            };

            int[][] icos_tiles_n = new[] {
                new [] {9, 4, 1, 6, 11}, new [] {4, 8, 10, 6, 0}, new [] {11, 7, 3, 5, 9},  new [] {2, 7, 10, 8, 5},
                new [] {9, 5, 8, 1, 0},  new [] {2, 3, 8, 4, 9},  new [] {0, 1, 10, 7, 11}, new [] {11, 6, 10, 3, 2},
                new [] {5, 3, 10, 1, 4}, new [] {2, 5, 4, 0, 11}, new [] {3, 7, 6, 1, 8},   new [] {7, 2, 9, 0, 6}
            };

            foreach (Tile t in grid.Tiles)
            {
                t.V = icos_tiles[t.Id];
                for (int k = 0; k < 5; k++)
                {
                    t.Tiles[k] = grid.Tiles[icos_tiles_n[t.Id][k]];
                }
            }

            for (int i = 0; i < 5; i++)
            {

                grid.AddCorner(i, 0, icos_tiles_n[0][(i + 4) % 5], icos_tiles_n[0][i]);
            }
            for (int i = 0; i < 5; i++)
            {

                grid.AddCorner(i + 5, 3, icos_tiles_n[3][(i + 4) % 5], icos_tiles_n[3][i]);
            }

            grid.AddCorner(10, 10, 1, 8);
            grid.AddCorner(11, 1, 10, 6);
            grid.AddCorner(12, 6, 10, 7);
            grid.AddCorner(13, 6, 7, 11);
            grid.AddCorner(14, 11, 7, 2);
            grid.AddCorner(15, 11, 2, 9);
            grid.AddCorner(16, 9, 2, 5);
            grid.AddCorner(17, 9, 5, 4);
            grid.AddCorner(18, 4, 5, 8);
            grid.AddCorner(19, 4, 8, 1);

            //_add corners to corners
            foreach (Corner c in grid.Corners)
            {
                for (int k = 0; k < 3; k++)
                {
                    c.Corners[k] = c.Tiles[k].Corners[(c.Tiles[k].GetCornerPosition(c) + 1) % 5];
                }
            }

            //new edges
            int next_edge_id = 0;
            foreach (Tile t in grid.Tiles)
            {
                for (int k = 0; k < 5; k++)
                {
                    if (t.Edges[k] == null)
                    {
                        grid.AddEdge(next_edge_id, t.Id, icos_tiles_n[t.Id][k]);
                        next_edge_id++;
                    }
                }
            }

            return grid;
        }

        private Grid CreateSubdividedGrid()
        {
            Grid grid = new Grid(Size + 1);

            int prev_tile_count = Tiles.Count;
            int prev_corner_count = Corners.Count;

            //old tiles
            for (int i = 0; i < prev_tile_count; i++)
            {
                grid.Tiles[i].V = Tiles[i].V;
                for (int k = 0; k < grid.Tiles[i].EdgeCount; k++)
                {
                    grid.Tiles[i].Tiles[k] = grid.Tiles[Tiles[i].Corners[k].Id + prev_tile_count];
                }
            }
            //old corners become tiles
            for (int i = 0; i < prev_corner_count; i++)
            {
                grid.Tiles[i + prev_tile_count].V = Corners[i].V;
                for (int k = 0; k < 3; k++)
                {
                    grid.Tiles[i + prev_tile_count].Tiles[2 * k] = grid.Tiles[Corners[i].Corners[k].Id + prev_tile_count];
                    grid.Tiles[i + prev_tile_count].Tiles[2 * k + 1] = grid.Tiles[Corners[i].Tiles[k].Id];
                }
            }
            //new corners
            int next_corner_id = 0;
            foreach (Tile n in Tiles)
            {
                Tile t = grid.Tiles[n.Id];
                for (int k = 0; k < t.EdgeCount; k++)
                {
                    grid.AddCorner(next_corner_id, t.Id, t.Tiles[(k + t.EdgeCount - 1) % t.EdgeCount].Id, t.Tiles[k].Id);
                    next_corner_id++;
                }
            }

            //connect corners
            foreach (Corner c in grid.Corners)
            {
                for (int k = 0; k < 3; k++)
                {
                    c.Corners[k] = c.Tiles[k].Corners[(c.Tiles[k].GetCornerPosition(c) + 1) % (c.Tiles[k].EdgeCount)];
                }
            }

            //new edges
            int next_edge_id = 0;
            foreach (Tile t in grid.Tiles)
            {
                for (int k = 0; k < t.EdgeCount; k++)
                {
                    if (t.Edges[k] == null)
                    {
                        grid.AddEdge(next_edge_id, t.Id, t.Tiles[k].Id);
                        next_edge_id++;
                    }
                }
            }

            return grid;
        }

        private void AddCorner(int id, int t1, int t2, int t3)
        {
            Corner c = Corners[id];
            Tile[] t = new[] { Tiles[t1], Tiles[t2], Tiles[t3] };
            Vector3 v = t[0].V + t[1].V + t[2].V;
            c.V = v.GetNormal();
            for (int i = 0; i < 3; i++)
            {
                t[i].Corners[t[i].GetTilePosition(t[(i + 2) % 3])] = c;
                c.Tiles[i] = t[i];
            }
        }

        private void AddEdge(int id, int t1, int t2)
        {
            Edge e = Edges[id];
            Tile[] t = new[] { Tiles[t1], Tiles[t2] };
            Corner[] c = new[] {
                Corners[t[0].Corners[ t[0].GetTilePosition(t[1]) ].Id],
                Corners[t[0].Corners[ (t[0].GetTilePosition(t[1]) + 1) % t[0].EdgeCount ].Id]
            };

            for (int i = 0; i < 2; i++)
            {
                t[i].Edges[t[i].GetTilePosition(t[(i + 1) % 2])] = e;
                e.Tiles[i] = t[i];
                c[i].Edges[c[i].GetCornerPosition(c[(i + 1) % 2])] = e;
                e.Corners[i] = c[i];
            }
        }
    }
}
