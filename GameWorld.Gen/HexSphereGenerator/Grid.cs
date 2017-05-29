using System;
using System.Diagnostics;
using VectorD = MathNet.Numerics.LinearAlgebra.Double.DenseVector;

namespace GameWorld.Gen.HexSphereGenerator
{
    internal class Grid<TTile, TCorner>
        where TTile : HexSphereTile, new()
        where TCorner : HexSphereTileCorner, new()
    {
        public int Size { get; }
        public TTile[] Tiles { get; }
        public TCorner[] Corners { get; }
        public Edge[] Edges { get; }

        public Grid(int size)
        {
            Size = size;

            Tiles = new TTile[GetTileCount(size)];
            Corners = new TCorner[GetCornerCount(size)];
            Edges = new Edge[GetEdgeCount(size)];

            for (int i = 0; i < Tiles.Length; i++)
            {
                Tiles[i] = new TTile();
                Tiles[i].Initialize(i, i < 12 ? 5 : 6);
            }

            for (int i = 0; i < Corners.Length; i++)
            {
                Corners[i] = new TCorner();
                Corners[i].Id = i;
            }

            for (int i = 0; i < Edges.Length; i++)
                Edges[i] = new Edge(i);
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

        public static Grid<TTile, TCorner> CreateSizeNGrid(int size)
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

        public static Grid<TTile, TCorner> CreateSize0Grid()
        {
            Grid<TTile, TCorner> grid = new Grid<TTile, TCorner>(0);
            double x = -0.525731112119133606;
            double z = -0.850650808352039932;

            VectorD[] icos_tiles = new[] {
                new VectorD(new double[] { -x, 0, z }), new VectorD(new double[] { x, 0, z }), new VectorD(new double[] { -x, 0, -z }), new VectorD(new double[] { x, 0, -z }),
                new VectorD(new double[] { 0, z, x }), new VectorD(new double[] { 0, z, -x }), new VectorD(new double[] { 0, -z, x }), new VectorD(new double[] { 0, -z, -x }),
                new VectorD(new double[] { z, x, 0 }), new VectorD(new double[] { -z, x, 0 }), new VectorD(new double[] { z, -x, 0 }), new VectorD(new double[] { -z, -x, 0 })
            };

            int[][] icos_tiles_n = new[] {
                new [] {9, 4, 1, 6, 11}, new [] {4, 8, 10, 6, 0}, new [] {11, 7, 3, 5, 9},  new [] {2, 7, 10, 8, 5},
                new [] {9, 5, 8, 1, 0},  new [] {2, 3, 8, 4, 9},  new [] {0, 1, 10, 7, 11}, new [] {11, 6, 10, 3, 2},
                new [] {5, 3, 10, 1, 4}, new [] {2, 5, 4, 0, 11}, new [] {3, 7, 6, 1, 8},   new [] {7, 2, 9, 0, 6}
            };

            foreach (TTile t in grid.Tiles)
            {
                t.TileCenterPosition = icos_tiles[t.Id];
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
            foreach (TCorner c in grid.Corners)
            {
                for (int k = 0; k < 3; k++)
                {
                    c.Corners[k] = c.Tiles[k].Corners[(c.Tiles[k].GetCornerPosition(c) + 1) % 5];
                }
            }

            //new edges
            int next_edge_id = 0;
            foreach (TTile t in grid.Tiles)
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

        private Grid<TTile, TCorner> CreateSubdividedGrid()
        {
            Grid<TTile, TCorner> grid = new Grid<TTile, TCorner>(Size + 1);

            int prev_tile_count = Tiles.Length;
            int prev_corner_count = Corners.Length;

            //old tiles
            for (int i = 0; i < prev_tile_count; i++)
            {
                grid.Tiles[i].TileCenterPosition = Tiles[i].TileCenterPosition;
                for (int k = 0; k < grid.Tiles[i].EdgeCount; k++)
                {
                    grid.Tiles[i].Tiles[k] = grid.Tiles[Tiles[i].Corners[k].Id + prev_tile_count];
                }
            }
            //old corners become tiles
            for (int i = 0; i < prev_corner_count; i++)
            {
                grid.Tiles[i + prev_tile_count].TileCenterPosition = Corners[i].V;
                for (int k = 0; k < 3; k++)
                {
                    grid.Tiles[i + prev_tile_count].Tiles[2 * k] = grid.Tiles[Corners[i].Corners[k].Id + prev_tile_count];
                    grid.Tiles[i + prev_tile_count].Tiles[2 * k + 1] = grid.Tiles[Corners[i].Tiles[k].Id];
                }
            }
            //new corners
            int next_corner_id = 0;
            foreach (TTile n in Tiles)
            {
                TTile t = grid.Tiles[n.Id];
                for (int k = 0; k < t.EdgeCount; k++)
                {
                    grid.AddCorner(next_corner_id, t.Id, t.Tiles[(k + t.EdgeCount - 1) % t.EdgeCount].Id, t.Tiles[k].Id);
                    next_corner_id++;
                }
            }

            //connect corners
            foreach (TCorner c in grid.Corners)
            {
                for (int k = 0; k < 3; k++)
                {
                    c.Corners[k] = c.Tiles[k].Corners[(c.Tiles[k].GetCornerPosition(c) + 1) % (c.Tiles[k].EdgeCount)];
                }
            }

            //new edges
            int next_edge_id = 0;
            foreach (TTile t in grid.Tiles)
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
            TCorner c = Corners[id];
            TTile[] t = new[] { Tiles[t1], Tiles[t2], Tiles[t3] };
            VectorD v = t[0].TileCenterPosition + t[1].TileCenterPosition + t[2].TileCenterPosition;


            c.V = (VectorD)v.Normalize(2.0);
            for (int i = 0; i < 3; i++)
            {
                t[i].Corners[t[i].GetTilePosition(t[(i + 2) % 3])] = c;
                c.Tiles[i] = t[i];
            }
        }

        private void AddEdge(int id, int t1, int t2)
        {
            Edge e = Edges[id];
            TTile[] t = new[] { Tiles[t1], Tiles[t2] };
            TCorner[] c = new[] {
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
