using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ObjectMenu : MonoBehaviour
{
    // first: select object
    public Canvas canvas;

    public float objHeightOnInstantiation;
    // second: drop the object at the center of the camera screen 
    public List<GameObject> interactableObjects;
    public ObjectMenuUI[] objects;
    // the position: Vector2 pixel space --> release a raycast, direct it at a 45 angle that starts at the center

    public float MenuUIWidth;
    public int fromObjMenu;
    public int toObjMenu;

    // we are assuming the game object is equal to 
    public void Awake()
    {
        InitializeAllObjects();
    }
    public void InitializeAllObjects()
    {
        string[] Prefabs = GetAllPrefabs();
        // https://forum.unity.com/threads/editor-want-to-check-all-prefabs-in-a-project-for-an-attached-monobehaviour.253149/
        // you can load all assets at a folder
        foreach(string prefab in Prefabs)
        {
            UnityEngine.Object o = AssetDatabase.LoadMainAssetAtPath(prefab);
            // check if the component exists 
            GameObject go;
            go = (GameObject)o;
            if (go.GetComponent<InteractableObject>())
            {
                interactableObjects.Add(go);
            }
        }
        // 
    }

    public void ConfigureMenuSettings()
    {
        float width = canvas.pixelRect.width;
        int amount = Mathf.FloorToInt(width / MenuUIWidth);

        objects = new ObjectMenuUI[amount];


    
    }

    public void OnMenuOpen()
    {
        // render all the         
        
        int index = 0;
        for(int i = fromObjMenu; i < toObjMenu; i++)
        {
            ObjectMenuUI objMenuUI = new ObjectMenuUI();
            // set up game object 
            objMenuUI.interactableObject = interactableObjects[i].GetComponent<InteractableObject>();
            objects[index] = objMenuUI;
            index += 1; 
        }
    }
    
    public void OnMenuClosed()
    {
        // do a for loop and destory 

    }


    // provoke when you click object 
    public void GenerateObject(InteractableObject obj)
    {
        Vector2 screenSpace = new Vector2(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenSpace);
        Vector3 direction = new Vector3(0,1,Mathf.Sqrt(2));
        Ray r = new Ray(worldPos, direction);

        RaycastHit hit;

        // can be ARPlane
        if(Physics.Raycast(r, out hit) && hit.collider.GetComponent<Floor>())
        {
            Vector3 newObjPos = new Vector3(hit.point.x,hit.point.y+objHeightOnInstantiation,hit.point.z);
            GameObject newObj = Instantiate(obj.gameObject, newObjPos, obj.gameObject.transform.rotation);
        }
    }

    public static string[] GetAllPrefabs()
    {
        // returns path names of assets
        string[] temp = AssetDatabase.GetAllAssetPaths();
        List<string> result = new List<string>();
        foreach (string s in temp)
        {
            if (s.Contains(".prefab")) result.Add(s);
        }
        return result.ToArray();
    }
}
