# cgeo
Implementation of some computational geometry algorithms.
## Convex hull
Graham scan.
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
