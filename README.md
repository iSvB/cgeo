# cgeo
Implementation of some computational geometry algorithms.

## Delaunay triangulation
Simple incremental algorithm with dynamic search cache.

1. Create superstructure.  
2. Perform step 3-4 for each node from input.  
3. Add node to triangulation.  
  1. Find triangle in which falls this node (or on rib).  
  2. If node lies in epsilon-neighborhood of any vertex of triangle - ignore this node.  
  3. If node fall on rib, then this rib splits on two new, and each triangle adjacent with this rib also splits in two new.
  4. If node falls in triangle - split this triangle in three new.  
4. Check Delaunay condition for new triangles and perform required changes.

![alt tag](https://raw.githubusercontent.com/iSvB/cgeo/master/Images/triangulation_1000.bmp)

## Convex hull
Graham scan.

1. Let *p0* - point from set of points **Q** with minimal Y-axis coordinate, or leftmost if exist matching.
2. Let *p1, p2, ..., pm* - rest of the points of **Q**, sorted by polar angle that measured counterclockwise relative to the point p0.
3. Add *p0* to hull.
4. Add *p1* to hull.
5. For each point *cur* in *p3..pm*:
  1. Let *a* - penultimate added point in hull.
  2. Let *b* - last added point in hull
  3. While axis *b-a* & *b-cur* forms left-handed coordinate system remove last added point from hull.
  4. Add *cur* to convex hull.

![alt tag](https://raw.githubusercontent.com/iSvB/cgeo/master/Images/hull-50.bmp)
