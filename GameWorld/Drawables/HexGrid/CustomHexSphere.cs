using GameWorld.Gen;
using Microsoft.Xna.Framework;
using MonoGameWorld.HexGrid;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameWorld.HexGrid
{
    // TODO: Name should be changed
    public class CustomHexSphere : HexSphere<CustomTile, CustomTileCorner>
    {
        private readonly long _gameTimeForFullDay = new TimeSpan(0, 0, 30).Ticks;

        private Perlin3D _perlin;

        private double _minNoise = double.MaxValue;
        private double _maxNoise = double.MinValue;

        private const int PerlinCoefficient = 80;

        public double WaterHeight { get; private set; } = 0.5f;
        private int desiredWaterCoveragePercent = 30;

        public Vector3 LightDirection { get; set; }

        public Matrix WorldMatrix { get; set; }

        public Quaternion AxisRotationQuaternion { get; private set; }

        private Vector3 _axisrotation;
        protected Vector3 AxisRotation
        {
            get { return _axisrotation; }
            set
            {
                _axisrotation = value;
                AxisRotationQuaternion = Quaternion.CreateFromYawPitchRoll(value.Y, value.X, value.Z);
                AxisRotationQuaternion.Normalize();
            }
        }

        public CustomHexSphere(int size) : base(size)
        {
            foreach (CustomTile t in Tiles)
            {
                t.ParentSphere = this;
            }

            _perlin = Perlin3D.Instance;

            foreach (var t in Tiles)
            {
                var val = _perlin.GetMultioctave3DNoiseValue(t.X * PerlinCoefficient, t.Y * PerlinCoefficient, t.Z * PerlinCoefficient, 1, 5, 1.5);

                _minNoise = Math.Min(_minNoise, val);
                _maxNoise = Math.Max(_maxNoise, val);
            }

            foreach (var t in Tiles)
            {
                var val = _perlin.GetMultioctave3DNoiseValue(t.X * PerlinCoefficient, t.Y * PerlinCoefficient, t.Z * PerlinCoefficient, 1, 5, 1.5);
                val = (val - _minNoise) / (_maxNoise - _minNoise);
                float fVal = (float)val;

                t.Height = val;
            }

            WaterHeight = GetHeightFromCoveragePercent(desiredWaterCoveragePercent);

            foreach (var t in Tiles)//separated water detection from height calculation for water level calculation
            {
                t.IsWater = t.Height < WaterHeight;
            }

            NorthPole.Color = Color.Red;
            SouthPole.Color = Color.Red;
        }

        public double GetHeightFromCoveragePercent(int coveragePercent)
        {
            double resultHeight = 0.5f;
            List<double> tiles_heights = new List<double>();

            foreach (var t in Tiles)
            {
                tiles_heights.Add(t.Height);
            }

            tiles_heights.Sort();

            if (coveragePercent < 0 || coveragePercent > 100)
            {
                resultHeight = 0.5f;
            }
            else if (coveragePercent == 0)
            {
                resultHeight = 0.0f;
            }
            else if (coveragePercent == 100)
            {
                resultHeight = 1.0f;
            }
            else
            {
                int index = (tiles_heights.Count * coveragePercent / 100) - 1;
                if (index > 0)
                {
                    resultHeight = tiles_heights[index];
                }
                else
                {
                    resultHeight = tiles_heights[0];
                }
            }

            return resultHeight;
        }

        public void Update(GameTime gameTime)
        {
            float timePassed = (float)gameTime.ElapsedGameTime.Ticks / _gameTimeForFullDay;

            AxisRotation = new Vector3(AxisRotation.X, AxisRotation.Y + timePassed, AxisRotation.Z);
        }
    }
}
