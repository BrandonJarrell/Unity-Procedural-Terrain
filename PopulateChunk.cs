using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulateChunk : MonoBehaviour
{
   public float chunkCellValue;
   // Start is called before the first frame update
   void Start()
   {

      /*****************
       * side node: calling other scripts functions in this functions start() will run those functions before their calss start() is executed
       * because this script in higher up in the list of attached scripts to the chunk... this is my guess
       ****************/

      //BUILDINGS   
      // Give populateChunk Script its population and tell it to call its buildings, thats done here to make sure its done in the right order
      transform.GetComponent<PopulateBuildings>().chunkPopulation = chunkCellValue;
      transform.GetComponent<PopulateBuildings>().buildBuildings();
   }
}
