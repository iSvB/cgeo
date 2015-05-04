namespace CGeo
{
    /// <summary>
    /// Represents two-dimensional position vector.
    /// </summary>
    public struct Vector
    {
        public double X;
        
        public double Y;
        
        public Vector(double X, double Y) : this()
        {
            this.X = X;
            this.Y = Y;
        }

        /// <summary>
        /// Initialise vector AB.
        /// </summary>
        /// <param name="A">Origin point.</param>
        /// <param name="B">Terminal point.</param>
        public Vector(Point A, Point B) : this()
        {
            Initialize(A, B);
        }
        
        /// <summary>
        /// Initialize position vector by origin and terminal points.
        /// </summary>
        /// <param name="A">Origin point.</param>
        /// <param name="B">Terminal point.</param>
        private void Initialize(Point A, Point B)
        {
            X = B.X - A.X;
            Y = B.Y - A.Y;
        }
    }
}
