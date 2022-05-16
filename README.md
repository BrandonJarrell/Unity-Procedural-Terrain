# Unity-Procedural-Terrain
This was a proof of concept for several game ideas I have. Procedural games in my mind provide a better replayability experience
and I like to capture this whenever possible.

# Functionality
The terrain is currently built using "chunks" similar to the concept of Minecraft chunks. This is for effeciency because the "view distance" or loading distance
can be adjusted and previously loaded chuncks can be disabled once rendering is too far. In Unity, they're currently Quads or Planes. I picked quads because there are only 2 geometrical shapes inside a quad (2 triangles) and are the easiest to process. They're not the best but they work to provide a proof of concept.

* ChunkManager     
I created a chunkManager as an empty GameObject, it keeps track of which chuncks currently exists and or are activated.  
I stored the GameObject chunks inside a Map datatype because I needed quick access to the chunks to activate and deactivate, which is the next best thing to an array of pointers. (C# does not have pointers unfortunately) The Map prevents me from having to scan the entire scene for a given chunk (inefficient).   
Each chunk is stored in the Map as a pair (String, GameObject) and get the GameObject (chunk) by player.transform.position.x/chunkWidth for the X and player.transform.position.y/chunkWidth for the Y, if returned false by the Map, create the chunk and add it to the map. 
These chuncks can also activate/generate other GameObjects once activated/generated. This keeps scene grouping simple and makes coding easy so that a chunk is incharge of every object created inside of it.   
The created chunk is supplied by a prefab, which can be any GameObject for easy modification. It will work with anything as long as the width is the same as the supplied with in the ChunkController








