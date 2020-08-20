# Spherical-cloud-populated-by-blue-spheres
The original project V0.1 has many issues:
  a- Blue spheres are instantiated and destroyed frequently which needs more CPU time and increases garbage collection tasks.
  b- every actor using LINQ statement which loops through the whole marble container more than one time.
  c- the logic makes Actors tend to clump up over time.

Solutions:
  a- Instead of saving Marbles in a dictionary, I saved it in a data structure for a fast search in 3D space called k-Dimensional Tree.
     this enhance performance perfectly.
  b- Instead of using instaniate and destroy functions, I used object pooling which meaning reuse of marbles and text on them.  (v0.2)
  c- No need to make public property in every actor which contains a copy of marbles container, We have one marble container so we can make it a singleton.
  d- We don't use physics so I disabled colliders.(V0.3)
