using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerChunkManager : MonoBehaviour
{

   // These variables maintain a grid-like style: (99 is 0) and (100 is 1)
   private int xRounded; // What the rounded X is 
   private int zRounded; // rounded Z
   public GameObject chunk;
   private float chunkWidth;
   //private Dictionary<string, GameObject> chunkMap = new Dictionary<string, GameObject>();
   private Dictionary<string, GameObject> chunkMap;
   private int maxDistance;


   private Transform ChunkManager; // parent object of all chunks
   

   void Start()
	{
      // maybe have the chunkwidth define the scale?
      chunkWidth = chunk.transform.localScale.x; 
      ChunkManager = GameObject.Find("ChunkManager").transform; // get the chunk manager

      chunkMap = ChunkManager.GetComponent<SceneController>().masterChunkMap;
      maxDistance = ChunkManager.GetComponent<SceneController>().maxDistance;

      // SET POSITION IN CHUNKS
      if (transform.position.x < 0) // fixes the negative direction offset, -1 and 1 share the same chunk
         xRounded = (int)((transform.position.x - chunkWidth) / chunkWidth);  // making the negative direction in x and z off by 1 entire chunk
      else
         xRounded = (int)(transform.position.x / chunkWidth);


      if (transform.position.z < 0) // fixes the negative direction offset, -1 and 1 share the same chunk
         zRounded = (int)((transform.position.z - chunkWidth) / chunkWidth);  // making the negative direction in x and z off by 1 entire chunk
      else
         zRounded = (int)(transform.position.z / chunkWidth);



      // Activate Chunks
      loadChunks();
	}

   void Update()
	{
      setChunkPosition(transform.position.x, transform.position.z);
	}


   /**************************************
   *  Updates the players chunk position and calls reloadChunks if there is a difference
   * ***********************************/
   public void setChunkPosition(float x, float z)
   {
     if (x < 0) // fixes the negative direction offset, -1 and 1 share the same chunk
         x -= chunkWidth;  // making the negative direction in x and z off by 1 entire chunk because the chunk lags one behind when negative
     if (z < 0)
         z -= chunkWidth;  // Try a better solution? this works for now

      int localXRounded = (int)(x / chunkWidth); // divide and cut off the decimal giving you your position
      int localZRounded = (int)(z / chunkWidth); // in chunk position

      if (localXRounded != xRounded || localZRounded != zRounded) // if the grid position is different, update
      {
         // Calculates the movement into -+ 1 in both x and Z;
         int xDifference = localXRounded - xRounded;  // putting these formulas directly into the calling of the function does NOT work oddly enough
         int zDifference = localZRounded - zRounded;
         
         xRounded = localXRounded;
         zRounded = localZRounded;

         // print(xRounded + " - " + localXRounded + " " + zRounded + " - " + localZRounded);
         reloadChunks(xDifference, zDifference);
      }
      return;
   }


   /***********************************************
   *  Makes a new chunk
   * ****************************************/
   void createChunk(int x, int z)
	{
      // Position for the chunk
      Vector3 location = new Vector3(((xRounded + x) * chunkWidth) + (chunkWidth / 2), 0f, ((zRounded) + z) * chunkWidth + (chunkWidth / 2));
      // make the chunk
      GameObject newChunk = Instantiate(chunk, location, Quaternion.identity, ChunkManager);
      // name the chunk
      newChunk.name = ((xRounded + x) + "," + (zRounded + z));
      // quads are naturally tilted because they're often used in 2D;
      newChunk.transform.Rotate(90f, 0, 0, Space.World);
      float chunkCellValue = ChunkManager.GetComponent<CellMapGenerator>().imageArray[(xRounded + x + maxDistance), (zRounded + z + maxDistance)];
      //newChunk.GetComponent<populateChunk>().chunkCellValue = 1 - chunkCellValue;
      
     newChunk.GetComponent<PopulateChunk>().chunkCellValue = 1 - ChunkManager.GetComponent<CellMapGenerator>().
       imageArray[(xRounded + x + maxDistance), (zRounded + z + maxDistance)];

  
      /////////////////COLOR/////////////////////
      //print((xRounded + x + maxDistance) + "," + (zRounded + z + maxDistance));
      newChunk.GetComponent<Renderer>().material.SetColor("_Color", Color.Lerp(Color.white, Color.black, 
        chunkCellValue));

      /*switch (Random.Range(1, 7))
      {
         case 1:
            newChunk.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            break;
         case 2:
            newChunk.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
            break;
         case 3:
            newChunk.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
            break;
         case 4:
            newChunk.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            break;
         case 5:
            newChunk.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
            break;
         default:
            newChunk.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
            break;
      }*/
      ///////////////////////COLOR/////////////////

      chunkMap.Add(newChunk.name, newChunk);
      newChunk.GetComponent<ChunkController>().currentPopulation = 1; 
      // setting it to 1 instead of using its increase function because I cant be certain what the value is without using a start() method inside its class
      return;
   }


   /*****************************
   * Loads the players chunk position when the player first exists
   **********************************/
   void loadChunks()
	{
      /******************************************************************
      * Only Runs when the player spawns, loads the 9 chunks around the user
      ******************************************************************/

      for (int z = -1; z < 2; z++)   // z axis
      {
         for (int x = -1; x < 2; x++)  // x axis
         {
            // if the chunk exists, activate it
            if (chunkMap.ContainsKey((xRounded + x) + "," + (zRounded + z)))
            {
               // PERFORMANCE find is "not effecient"
               //ChunkManager.transform.Find((xRounded + x) + "," + (zRounded + z)).gameObject.SetActive(true);
               chunkMap[((xRounded + x) + "," + (zRounded + z))].GetComponent<ChunkController>().increasePopulation();
            }
            else // IF IT DOESN'T EXIST, MAKE IT
            {
               createChunk(x,z);
            }

         }
      }
      return;
   }



   /***********************************
   *  Takes a -1 or +1 in x and z and determines 
   *  which set of chunks need to be loaded or unloaded
   * ***********************************/
   private void reloadChunks(int xDifference, int zDifference) 
   {
      /*******************
       * Controls which chunks are active, if there aren't any, they will be created.
       ********************/

      /*
       * Math for calculating the name goes as follows
       * 
       * xRounded = players position in chunks.  
       * ((int)(player.position.x / chunkWdith) this calculation turns an x positon of 234, to 2.34 (assuming chunk width is 100)
       * then casting it to an int gets rid of any decimal points.. so 200 - 299 is all inside chunk 2 of whateevr plane 
       * (yes its dynamic in accordance with the chunk width)
       * 
       * x or z Difference is basically +1 or -1. when the player crosses a boundary from chunk 2 to chunk 3,
       * it will be +1.   thus, the position of the player now is xRounded + xDifference.  this applies to the x or z, depending on which one.
       * thus, a for loop looping through 3 times on the correct plane will switch the front/back to turn on/off, and that what w is for.
       * 
       * now, 4 directions are a little different because of where w is placed and where the +/- 3 is placed and there we go!   I manually added the +/- 3 to deactivate
       * the chunks opposite to the direction of the player's new tile location.  
       * 
       * I DO NOT MODIFY XROUNDED OR ZROUNDED HERE because setChunkPosition does that and acts as a if statement barrier for effeciency.
       * 
       * I do not know of a better way to mathmatically just know which direction the user is going, so I did 4 if Statements and thats what the differences originally
       * were for, but they also made these for loops easier to the brain
       */


      ///// MOVE IN THE - X /////////////////////////////
      if (xDifference < 0) 
		{
         print("moving - x");

         // The - Row because -1 (xDifference) from the direction the unit is advancing
         for (int w = -1; w < 2; w++)
         {
                     // (player.chunk position + chunk direction moved (-1 in this case)) , (player.chunk position + -1,0,1 tile to be activated/deactivated)
            if (chunkMap.ContainsKey((xRounded + xDifference) + "," + (zRounded + w))) 
            {
               chunkMap[((xRounded + xDifference) + "," + (zRounded + w))].GetComponent<ChunkController>().increasePopulation();
            }
            else
            {
               createChunk((xDifference), (w));
            }
         }

         // The + Row because +3 from the direction the unit is advancing
         for (int w = -1; w < 2; w++)
         {   // long but helpful translation
          // (player.chunk position + chunk direction moved (-1 in this case) + the distance from the back chunks that need to be turned off) , (player.chunk position + -1,0,1 tile to be activated/deactivated)
            if (chunkMap.ContainsKey((xRounded + xDifference + 3) + "," + (zRounded + w))) // + 3 because thats 3 rows away from whats being activated
            {
               chunkMap[((xRounded + xDifference + 3) + "," + (zRounded + w))].GetComponent<ChunkController>().decreasePopulation();
            }
            else
            {
               createChunk((xDifference + 3), (w));
            }

         }
         return;
		}


      //////////// MOVE IN THE + X///////////////////////////////
      if (xDifference > 0) 
      {
         print("moving + x");

         // The + Row because +1 (xDifference) from the direction the unit is advancing
         for (int w = -1; w < 2; w++)
         {
            if (chunkMap.ContainsKey((xRounded + xDifference) + "," + (zRounded + w)))
            {
               chunkMap[((xRounded + xDifference) + "," + (zRounded + w))].GetComponent<ChunkController>().increasePopulation();
            }
            else
            {
               createChunk((xDifference), (w));
            }
         }

         // The - Row because -3 from the direction the unit is advancing
         for (int w = -1; w < 2; w++)
         {
            if (chunkMap.ContainsKey((xRounded + xDifference - 3) + "," + (zRounded + w))) // + 3 because thats 3 rows away from whats being activated
            {
               chunkMap[((xRounded + xDifference - 3) + "," + (zRounded + w))].GetComponent<ChunkController>().decreasePopulation();
            }
            else
            {
               createChunk((xDifference - 3), (w));
            }

         }
         return;
      }


      ///////////// MOVE IN THE - Z////////////////////////////
      if (zDifference < 0) 
      {
         print("moving - z");

         // The - Row because -1 (zDifference) from the direction the unit is advancing
         for (int w = -1; w < 2; w++) 
         {
            if (chunkMap.ContainsKey((xRounded + w) + "," + (zRounded + zDifference)))
            {
               chunkMap[((xRounded + w) + "," + (zRounded + zDifference))].GetComponent<ChunkController>().increasePopulation();
            }
            else
            {
               createChunk((w), (zDifference));
            }
         }

         // The + Row because +3 from the direction the unit is advancing
         for (int w = -1; w < 2; w++)
         {
            if (chunkMap.ContainsKey((xRounded + w) + "," + (zRounded + zDifference + 3))) // + 3 because thats 3 rows away from whats being activated
            {
               chunkMap[((xRounded + w) + "," + (zRounded + zDifference + 3))].GetComponent<ChunkController>().decreasePopulation();
            }
            else
            {
               createChunk((w), (zDifference + 3));
            }
            
         }
         return;
      }

      //////////////// MOVE IN THE + Z///////////////////////////////////
      if (zDifference > 0) 
      {
         print("moving + z");

         // The + Row because +1 (zDifference) from the direction the unit is advancing
         for (int w = -1; w < 2; w++)   
         {
            if (chunkMap.ContainsKey((xRounded + w) + "," + (zRounded + zDifference)))
            {
               chunkMap[((xRounded + w) + "," + (zRounded + zDifference))].GetComponent<ChunkController>().increasePopulation();
            }
            else
            {
               createChunk((w), (zDifference));
            }
         }

         // The - Row because -3 from the direction the unit is advancing
         for (int w = -1; w < 2; w++)  
         {
            if (chunkMap.ContainsKey((xRounded + w) + "," + (zRounded + zDifference - 3)))
            {
               chunkMap[((xRounded + w) + "," + (zRounded + zDifference - 3))].GetComponent<ChunkController>().decreasePopulation();
            }
            else
            {
               createChunk((w), (zDifference - 3));
            }
         }
         return;
      }

      // END OF FUNCTION
      return;
   }
}
