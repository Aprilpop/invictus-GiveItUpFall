using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ReplaceGameObjects : ScriptableWizard
{
	public bool copyValues = true;
    public bool clearNewObjects = false;
	public List<GameObject> newObjects;
	//public GameObject[] OldObjects;
	public bool clearOldObjects = false;
	public List <GameObject> OldObjects;

    [MenuItem("Custom/Replace GameObjects")]


	static void CreateWizard()
	{
		ScriptableWizard.DisplayWizard("Replace GameObjects", typeof(ReplaceGameObjects), "Replace", "Replace again");
	}

	void OnWizardCreate()
	{
        for (int i = 0; i < OldObjects.Count; i++)
        {        
			GameObject newObject;
			newObject = (GameObject)PrefabUtility.InstantiatePrefab(RandomObject());
			newObject.transform.position = OldObjects[i].transform.position;
			newObject.transform.rotation = OldObjects[i].transform.rotation;
			newObject.transform.parent = OldObjects[i].transform.parent;

			if(clearOldObjects)
			{
				DestroyImmediate(OldObjects[i]);
			}
			else
			{
				//	DestroyImmediate(go);
				int index = OldObjects.IndexOf(OldObjects[i]);
				DestroyImmediate(OldObjects[i]);
				OldObjects[index] = newObject;
				
			}
           
    

		}
		if (clearOldObjects)
			OldObjects.Clear();
        if (clearNewObjects)
            newObjects = null;

    }
	/*
	void OnWizardUpdate()
	{
		OldObjects = new List<GameObject>();
		if (Selection.gameObjects.Length > 0)
		{
			//	Debug.Log("!!!");
			foreach (GameObject so in Selection.gameObjects)
			{
				OldObjects.Add(so);

			}
		}
	}
	*/
	void OnWizardOtherButton()
	{
		OnWizardCreate();
	}

	GameObject RandomObject()
	{
		if (newObjects.Count > 0)
		{
			return newObjects[Random.Range(0, newObjects.Count)];
		}
		else return null;
	}
}