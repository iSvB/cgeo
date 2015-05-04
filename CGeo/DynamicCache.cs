using System;
using System.Linq;

namespace CGeo
{
    /// <summary>
    /// Represents cache of dynamic size (depends on amount of elements). 
    /// </summary>
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
        // Cache table.        
        private Triangle[][] cache;

        #endregion
        #region Methods
        #region Public 

        /// <summary>
        /// Increment count of nodes.
        /// </summary>
        public void IncrementNodeCount(int count)
        {
            n += count;
            if (n >= limit)
                IncreaseSize();
        }

        /// <summary>
        /// Initializes cache of size 2x2 with passed triangles.
        /// </summary>
        public void Initialize(Triangle t00, Triangle t01, Triangle t10, Triangle t11)
        {
            cache[0][0] = t00;
            cache[0][1] = t01;
            cache[1][0] = t10;
            cache[1][1] = t11;
        }

        /// <summary>
        /// Update most suitable cache cell with passed triangle.
        /// </summary>
        /// <param name="T"></param>
        public void Update(Triangle T)
        {
            var x = T.Vertices.Sum(p => p.X) / 3;
            var y = T.Vertices.Sum(p => p.Y) / 3;
            int row = GetRow(y);
            int col = GetCol(x);
            cache[row][col] = T;
        }

        /// <summary>
        /// Gets triangle from cache by passed node.
        /// </summary>
        /// <returns>Triagnle that is located close to passed node.</returns>
        public Triangle Get(Point node)
        {
            int row = GetRow(node.Y);
            int col = GetCol(node.X);
            return cache[row][col];
        }

        #endregion
        #region Private
        #endregion

        /// <summary>
        /// Gets index of cache's column by passed value.
        /// </summary>
        /// <returns>Index of column in cache table.</returns>
        private int GetCol(double value)
        {
            return (int)Math.Floor((value - minX) / xUnitSize);
        }

        /// <summary>
        /// Gets index of cache's row by passed value.
        /// </summary>
        /// <returns>Index of row in cache table.</returns>
        private int GetRow(double value)
        {
            return (int)Math.Floor((value - minY) / yUnitSize);
        }

        /// <summary>
        /// Increase size for cache table.
        /// </summary>
        private void IncreaseSize()
        {           
            var newSize = m * 2;
            var newCache = new Triangle[newSize][];
            for (int i = 0; i < newSize; ++i)
                newCache[i] = new Triangle[newSize];
            // Assign c[x,y] to c[2x,2y],c[2x+1,2y],c[2x,2y+1],c[2x+1,2y+1].
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

        /// <summary>
        /// Update size of cell in cache table.
        /// </summary>
        private void UpdateUnitSize()
        {
            xUnitSize = (maxX - minX) / m;
            yUnitSize = (maxY - minY) / m;
        }

        #endregion
        #region Constructors

        /// <summary>
        /// Creates instance of <code>DynamicCache</code> class.
        /// </summary>
        /// <param name="growthRate">Rate of growth for cache table.</param>
        /// <param name="minX">Minimal value for X-axis.</param>
        /// <param name="maxX">Maximal value for X-axis.</param>
        /// <param name="minY">Minimal value for Y-axis.</param>
        /// <param name="maxY">Maximum value for Y-axis.</param>
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
