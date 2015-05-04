namespace CGeo
{
    /// <summary>
    /// Point in two-dimensional cartesian coordinate system.
    /// </summary>
    public struct Point
    {        
        public double X;

        public double Y;

        public Point(double X, double Y)
            : this()
        {
            this.X = X;
            this.Y = Y;
        }

        /// <summary>
        /// Determines, whether point located in epsilon-neighborhood of point <code>p</code> or not.
        /// </summary>
        /// <returns>
        /// True - if this point located in epsilon-neighborhood of point <code>p</code>, otherwise - false.
        /// </returns>
        public bool IsInEpsilonArea(Point p)
        {
            if (p.X.IsInEpsilonArea(X) && p.Y.IsInEpsilonArea(Y))
                return true;
            return false;
        }
    }
}
