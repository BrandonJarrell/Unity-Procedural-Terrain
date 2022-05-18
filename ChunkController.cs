using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkController : MonoBehaviour
{
	public int currentPopulation; 
	// NOT set to 0 through this scripts Start() because apparently an instantiated object's start() doesnt get called before it moves on in the function that instantiated this (ya know, the procedural way)
	// thus, using Start() overwrites the other functions efforts... just pray the chunks population number start at 0 when the scene is loaded (TEMPORARY)


	public void increasePopulation()
	{
		if(!this.gameObject.activeInHierarchy)
			this.gameObject.SetActive(true);
		++currentPopulation;
	}

	public void decreasePopulation()
	{
		--currentPopulation;
		if (currentPopulation < 1)
			this.gameObject.SetActive(false);

	}
}
