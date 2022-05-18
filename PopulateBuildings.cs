using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulateBuildings : MonoBehaviour
{

   // the maximum area we're allowed to play with
   private int objectWidth;
   private bool[] buildingPositions = new bool[9];

   // information comes from the cell map inside the ChunkManager
   public float chunkPopulation;

   // Start is called before the first frame update
   void Start()
   {
      
      // calling this here doesn't do it in time for generateFoundation().. so its called down there too
      objectWidth = (int)this.transform.localScale.x;
      // for loop could be redundant because they're all automatically set to false?
      for (int i = 0; i < 9; i++)
      {
         buildingPositions[i] = false;
      }
   }

   public void buildBuildings()
	{
      
      int tempChunkPopulation = (int)(chunkPopulation * 10);
      //print(chunkPopulation + " is original, vs: " + tempChunkPopulation);
      switch (tempChunkPopulation)
      {
         case 0:
            if (Random.Range(0, 10) == 0) // 10%    // 1 MAX
               generateFoundation();
            break;
         case 1:
            if (Random.Range(0, 8) == 0) // 12.5%    // 1 MAX
               generateFoundation();
            break;
         case 2:
            if (Random.Range(0, 4) == 0) // 25%%     // 1 MAX
               generateFoundation();
            break;
         case 3:
            for (int i = 0; i < Random.Range(1, 3); i++) // 100% for 1, 50% for a second  // 2 MAX
            {
               generateFoundation();
            }
            break;
         case 4:
            for (int i = 0; i < Random.Range(1, 4); i++) // 100% to spawn 1, 66% for 2, 33 for 3  // 3 MAX
            {
               generateFoundation();
            }
            break;
         case 5:
            for (int i = 0; i < Random.Range(2, 5); i++) // 100% to spawn 2, 66% for 3, 33 for 4 // 4 MAX
            {
               generateFoundation();
            }
            break;
         case 6:
            for (int i = 0; i < Random.Range(2, 5); i++) // 100% to spawn 2, 66% for 3, 33 for 4  // 8 MAX but likely 5
            {
               generateFoundation();
               if (Random.Range(0, 10) == 0)  // + 10% to spawn another each time
                  generateFoundation();
            }
            break;
         case 7:
            for (int i = 0; i < Random.Range(4, 6); i++) // 100% to spawn 4, 50% for 5     // 9 Max, unknown likely
            {
               generateFoundation();
               if (Random.Range(0, 10) == 0)  // + 10% to spawn another each time
                  generateFoundation();
            }
            break;
         case 8:
            for (int i = 0; i < Random.Range(5, 8); i++) // 100% to spawn 5, 66% for 6, 33 for 7 
            {
               generateFoundation();
               if (Random.Range(0, 10) == 0)  // + 10% to spawn another each time
                  generateFoundation();
            }
            break;
         case 9:
            for (int i = 0; i < Random.Range(6, 9); i++) // 100% to spawn 6, 66% for 7, 33 for 8 
            {
               generateFoundation();
               if (Random.Range(0, 10) == 0)  // + 10% to spawn another each time
                  generateFoundation();
            }
            break;
         case 10:
            for (int i = 0; i < 9; i++) // 100% to spawn 9
            {
               generateFoundation();
            }
            break;
      }
   }


   void generateFoundation()
   {
      // performed in Start() as well because this is called before start is called. I'm guessing thats because the other script is above in the hierarchy
      objectWidth = (int)this.transform.localScale.x;

      // get the chunks position
     int xPosition = (int)transform.position.x;
      int zPosition = (int)transform.position.z;

      // generate a random number for the width and height of the house  (size of the chunk scales the house width [where the 10 and 4 come from])
      int xWidth = (int)Random.Range((objectWidth / 12), (objectWidth / 6));
      int zWidth = (int)Random.Range((objectWidth / 12), (objectWidth / 6));


      ////////////////////////   DETERMINES WHICH POSITIONS IN GRID ARE AVAIABLE OR TAKEN ////////////////////////////////////
      int randomX = 0;// = Random.Range(-1, 2);
      int randomZ = 0; //= Random.Range(-1, 2);
      int positionArray = 0;
      int counter = 0;
      do
      {
         // set the randoom number to be tested. 0 is inclusive, 9 isnt..?
         positionArray = Random.Range(0, 9);
         // check how many times we've looped through, if done 8 times, individually loop through all of them.
         if (counter > 8)
            for (int i = 0; i < 9; i++)
            {
               if (buildingPositions[i] == false) // if found an empty spot
               {
                  positionArray = i;
                  break; // if found, break the for loop
               }
               else if (i == 8) // else if we have looped through every spot and still nothing
               {
                  print("Positions are full in this Chunk");
                  return;
               }
            }

         // preset positions on a 3x3 grid based on a 0-8 grid
         switch (positionArray)
         {
            case 0:
               randomX = -1 * (objectWidth / 3);
               randomZ = -1 * (objectWidth / 3);
               break;
            case 1:
               randomZ = -1 * (objectWidth / 3); // x is already 0
               break;
            case 2:
               randomX = 1 * (objectWidth / 3);
               randomZ = -1 * (objectWidth / 3);
               break;
            case 3:
               randomX = -1 * (objectWidth / 3); // z is already 0
               break;
            //case 4:
               // both are already set to 0
               //break;
            case 5:
               randomX = 1 * (objectWidth / 3);
               break;
            case 6:
               randomX = -1 * (objectWidth / 3);
               randomZ = 1 * (objectWidth / 3);
               break;
            case 7:
               randomZ = 1 * (objectWidth / 3);
               break;
            case 8:
               randomX = 1 * (objectWidth / 3);
               randomZ = 1 * (objectWidth / 3);
               break;
         }
         counter++;
         if (counter > 10)
            return;
      } while (buildingPositions[positionArray] != false);
      //////////////////////////////////////////////////////////////////////////////////////////////


      // keeps track of which of the 9 positions within a single chunk are occupied
      buildingPositions[positionArray] = true;

      // create the position
      Vector3 position = new Vector3(randomX + xPosition, 0f, randomZ + zPosition);

      // create the object and set its position
      GameObject main = GameObject.CreatePrimitive(PrimitiveType.Cube);
      main.transform.position = position;

      // shape of the cube
      main.transform.localScale += new Vector3((xWidth), 0.5f, (zWidth));

      // assign the chunk as the cubes parent
      main.transform.SetParent(this.transform);


   }
}
