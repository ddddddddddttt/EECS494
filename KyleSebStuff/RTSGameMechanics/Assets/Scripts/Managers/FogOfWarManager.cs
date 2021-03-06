﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Pathfinding;
using RTS;

public static class FogOfWarManager {

	public static int playerID;
	
	private static int planeSizeX;
	private static int planeSizeZ;
	private static int planeAmount;
	private static float fogHeight;
	
	private static List<List<GameObject>> gridOfFog;
	
	private static GameObject fogObject;
	
	private static int gridRows;
	private static int gridColumns;
	
	public static void Init() {
		fogObject = (GameObject) Resources.Load("FogOfWar/FogTile");
		
		planeAmount = 5;
		fogHeight = 0f;
		planeSizeX = (int) (RTSGameMechanics.GetMapSizes().x / planeAmount);
		planeSizeZ = (int) (RTSGameMechanics.GetMapSizes().z / planeAmount);
		
		gridOfFog = new List<List<GameObject>>();
		
		int initialX = planeSizeX / 2;
		int initialZ = planeSizeZ / 2;
		for (int i = 0; i < planeAmount; i++) {
			List<GameObject> innerList = new List<GameObject>();
			for(int j = 0; j < planeAmount; j++) {
				Vector3 pos = new Vector3(initialX + j*planeSizeX, fogHeight, initialZ + i*planeSizeZ);
				GameObject fogTile = GameObject.Instantiate (fogObject, pos, Quaternion.identity) as GameObject;
				innerList.Add(fogTile);
			}
			gridOfFog.Add(innerList);
		}
	}

	public static GameObject getMyFogTile(Int3 pos) {
		int zCord = pos.z / planeSizeZ / Int3.Precision;
		int xCord = pos.x / planeSizeX / Int3.Precision;
		return gridOfFog[zCord][xCord];
	}

	public static void updateFogTileUnitCount(GameObject oldFogTile, GameObject newFogTile, int pID) {
		if (oldFogTile != null) {
			removeUnitFromFogTile(oldFogTile, pID);
		}

		if(newFogTile != null) {
			addUnitToFogTile(newFogTile, pID);
		}
	}

	private static void removeUnitFromFogTile(GameObject fogTile, int pID) {
		if(pID == playerID) {
			if(fogTile.GetComponent<FogScript>().friendlyUnitCount != 0) {
				fogTile.GetComponent<FogScript>().friendlyUnitCount--;
			}
		}
		else {
			if(fogTile.GetComponent<FogScript>().enemyUnitCount != 0) {
				fogTile.GetComponent<FogScript>().enemyUnitCount--;
			}
		}
	}

	private static void addUnitToFogTile(GameObject fogTile, int pID) {
		if(pID == playerID) {
			fogTile.GetComponent<FogScript>().friendlyUnitCount++;
		}
		else {
			fogTile.GetComponent<FogScript>().enemyUnitCount++;
		}
	}

	public static bool isVisible(GameObject fogTile, int ID) {
		int count = fogTile.GetComponent<FogScript> ().friendlyUnitCount;

		if(ID == playerID && count > 0) {
			//always show friendly units
			return true;
		}
		else if(count > 0) {
			//show enemey units if you have friendly units in same fog tile
			return true;
		}
		else {
			//no friendlies in fog tile so hide enemey units
			return false;
		}
	}

}
