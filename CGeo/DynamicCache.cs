using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGeo
{
    public class DynamicCache
    {
        #region Fields

        // Max & min values.
        private readonly double minX;
        private readonly double maxX;
        private readonly double minY;
        private readonly double maxY;
        // Rate of growth.
        private readonly int growthRate;
        // Size of cache unit.
        private double xUnitSize;
        private double yUnitSize;
        // Size of cache.
        private int m = 2;
        // When n equals to the limit we should increase cache size.
        private int limit;
        // Count of nodes.
        private int n = 0;        
        private Triangle[][] cache;

        #endregion
        #region Methods

        /// <summary>
        /// Increment count of nodes.
        /// </summary>
        public void IncrementNodeCount(int count)
        {
            n += count;
            if (n >= limit)
                IncreaseSize();
        }

        public void Initialize(Triangle t00, Triangle t01, Triangle t10, Triangle t11)
        {
            cache[0][0] = t00;
            cache[0][1] = t01;
            cache[1][0] = t10;
            cache[1][1] = t11;
        }
        
        public void Update(Triangle T)
        {
            var x = T.Points.Sum(p => p.X) / 3;
            var y = T.Points.Sum(p => p.Y) / 3;
            int row = GetRow(y);
            int col = GetCol(x);
            cache[row][col] = T;            
        }

        public Triangle Get(Point node)
        {
            int row = GetRow(node.Y);
            int col = GetCol(node.X);
            return cache[row][col];
        }

        private int GetCol(double value)
        {
            return (int)Math.Floor((value - minX) / xUnitSize);
        }

        private int GetRow(double value)
        {
            return (int)Math.Floor((value - minY) / yUnitSize);
        }

        private void IncreaseSize()
        {
            var newSize = m * 2;
            var newCache = new Triangle[newSize][];
            for (int i = 0; i < newSize; ++i)
                newCache[i] = new Triangle[newSize];
            for (int i = 0; i < m; ++i)
                for (int j = 0; j < m; ++j)
                {
                    var doubleI = i * 2;
                    var doubleJ = j * 2;
                    var value = cache[i][j];
                    newCache[doubleI][doubleJ] = value;
                    newCache[doubleI][doubleJ + 1] = value;
                    newCache[doubleI + 1][doubleJ] = value;
                    newCache[doubleI + 1][doubleJ + 1] = value;
                }
            cache = newCache;
            m = newSize;
            limit = growthRate * m * m;
            UpdateUnitSize();
        }

        private void UpdateUnitSize()
        {
            xUnitSize = (maxX - minX) / m;
            yUnitSize = (maxY - minY) / m;
        }

        #endregion
        #region Constructors

        public DynamicCache(int growthRate, double minX, double maxX, double minY, double maxY)
        {
            this.minX = minX;
            this.maxX = maxX;
            this.minY = minY;
            this.maxY = maxY;
            UpdateUnitSize();
            this.growthRate = growthRate;
            cache = new Triangle[m][];
            for (int i = 0; i < m; ++i)
                cache[i] = new Triangle[m];
            limit = growthRate * m * m;
        }

        #endregion
    }
}
