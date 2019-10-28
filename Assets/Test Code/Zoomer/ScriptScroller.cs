using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptScroller : MonoBehaviour
{
    // Start is called before the first frame update
    private Mesh mesh;
    private Vector2[] meshUVs;
    private Renderer rend;

    private Vector2[] uvsToMove;
    private float currScaleX = 1;
    private float currScaleY = 1; 
    void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        meshUVs = mesh.uv;

        rend = GetComponent<Renderer>();

        printMeshUVS();
        retrieveMeshForUVMovement();
       
    }

    void printMeshUVS()
    {
        foreach(Vector2 uvCoord in meshUVs)
        {
            Debug.Log(uvCoord);
        }
    }

    // very specific to this prefab lol
    void retrieveMeshForUVMovement()
    {
        uvsToMove = new Vector2[4];
        uvsToMove[0] = meshUVs[8];
        uvsToMove[1] = meshUVs[9];
        uvsToMove[2] = meshUVs[10];
        uvsToMove[3] = meshUVs[11];
    }

    // Update is called once per frame
    void Update()
    {

        uvsToMove[0].x += Time.deltaTime;
        uvsToMove[1].x += Time.deltaTime;
        uvsToMove[2].x += Time.deltaTime;
        uvsToMove[3].x += Time.deltaTime;

        meshUVs[8] = uvsToMove[0];
        meshUVs[9] = uvsToMove[1];
        meshUVs[10] = uvsToMove[2];
        meshUVs[11] = uvsToMove[3];

        mesh.uv = meshUVs;

    }

    // problems to consider => (limit to 15 seconds) 

    // matching the markers with
    // where to clamp 

    // assume the texture jpeg is always going to be a square (set up through ProBuilder)

    // each time you scroll left, you will pass by 15

    // when the last two right coordinates hit x = 15, you cant scroll right anymore; 
    // when the last two left coordinates hit x = 0, you can't scroll left anymore;

    // use the uv coordinates as a clamp to determine which
    // keyframe Points to render each time you zoom in or zoom out
    // or scroll

    // invoke through a button 
    // use Material.SetTextureScale
    public void zoomIn()
    {
        // come up with the math formula to manipulate the 
        currScaleX -= 0.25f;
        currScaleY -= 0.25f;
        rend.material.SetTextureScale("_MainTex", new Vector2(currScaleX, currScaleY));
    }

    // invoke through a button 
    public void zoomOut()
    {
        currScaleX += 0.25f;
        currScaleY += 0.25f;
        rend.material.SetTextureScale("_MainTex", new Vector2(currScaleX, currScaleY));
        // come up with the math formula to manipulate the 
    }
}
