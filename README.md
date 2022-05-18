# Unity-Procedural-Terrain
This was a proof of concept for several game ideas I have. Procedural games in my mind provide a better replayability experience
and I like to capture this whenever possible.        
- I left my commented out code and notes so help convey all information I have

# Functionality
The terrain is currently built using "chunks" similar to the concept of Minecraft chunks. This is for effeciency because the "view distance" or loading distance
can be adjusted and previously loaded chuncks can be disabled once rendering is too far. In Unity, they're currently Quads or Planes. I picked quads because there are only 2 geometrical shapes inside a quad (2 triangles) and are the easiest to process. They're not the best but they work to provide a proof of concept.

* Player Chunk Manager     
I created a chunkManager as an empty GameObject, it keeps track of which chuncks currently exists and or are activated.  
I stored the GameObject chunks inside a Map datatype because I needed quick access to the chunks to activate and deactivate, which is the next best thing to an array of pointers. (C# does not have pointers unfortunately) The Map prevents me from having to scan the entire scene for a given chunk (inefficient).   
Each chunk is stored in the Map as a pair (String, GameObject) and get the GameObject (chunk) by player.transform.position.x/chunkWidth for the X and player.transform.position.y/chunkWidth for the Y, if returned false by the Map, create the chunk and add it to the map. 
These chuncks can also activate/generate other GameObjects once activated/generated. This keeps scene grouping simple and makes coding easy so that a chunk is incharge of every object created inside of it.   
The created chunk is supplied by a prefab, which can be any GameObject for easy modification. It will work with anything as long as the width is the same as the supplied with in the ChunkController    
The chunks have a population value, this allows multiple players to correctly occupy the same chunks and when one player leaves, it does not disable the map below the other player.

![Image of Game](https://github.com/BrandonJarrell/Unity-Procedural-Terrain/blob/main/player_movement.gif)

* Cell Noise Map     
I've included a previous project that impliments a cell noise map to allow each chunk to have flow of density (example: Population/density of buildings, resources or whatever) between the chunks. This prevents one chunk with maximum density to be right next to a empty chunk. The current issue with this idea is that once the player goes beyond the set width of the cellmap (each chunk being a single value inside of the 2d array which stored the cell map pixel values) the value returns the same every time. It's possible to wrap it all, but I haven't decided if I want that in my games. Even Minecraft has a limit.

[Cell Noise Map Project](https://github.com/BrandonJarrell/Unity-CellNoiseMap)            
![Image of Game](https://github.com/BrandonJarrell/Unity-CellNoiseMap/blob/main/CellNoiseMap.gif)


* Populate Buildings      
I wanted procedural buildings too, I started the process and the building quantity is based on the chunks cell value in acoordance with the cell noise map value.    
My goal is that the buildings origin cube is place, its width and length are randomized. Then there is a chance rolled to add another slightly smaller randomized cude/rectangle touching it so it is unique. the process is completely based off of its chunk population however it still has a bit of randomness too it allowing you to find buildings out in the middle of nowhere. The issue is that its a mathmatical nightmare to make sure no building goes over its boundaries and overlap another chunk OR overlaps another building. It is completely possible, I just have other projects that I'm more interested it and I will do that AFTER I set in stone how my terrain will work and its width of the chunks. The buildings would be in charge of generating themselves based on the foundations I give them.







