using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoomer : MonoBehaviour
{
    private Mesh m;
    // the uv coordinates won't change
    private Vector2[,] markerPixelCoordinates;
    private Vector2[,] spacingCoordinates;

    public Texture2D texture2D;

    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector2[] uvs = new Vector2[vertices.Length];

       for(int i = 0; i < mesh.uv.Length; i++)
        {
            Debug.Log("mesh uv coordinates " + mesh.uv[i]);
        }

       for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
            Debug.Log("uv coords " + uvs[i]);
        }
        mesh.uv = uvs;
    }

    // [---------] [|] [---------] [|] [----------] [|] [---------] [|] [---------]
    // have a midpoint between the spacing for expansion purposes 
    // have a displacement formula that pushes the marker left
    // pushes the marker right

    // when zooming, the coordinates are moving relative to center of the timeline. those left of the center will move left. those right of the center will move right.
    // center is defined based on which coordinate, in pixel space, 
    // is touching the timeline center, which is in timeline local space.

    // 1st step -> // 
    // invoke the zoom in / zoom out method

    // second thing --> retrieve all the markers

    // like the mesh vertices array assignment
    // recalcuate the new positions of the markers
    // assign the new white pixels colors for displaced markers (left / right)
    // then -> move the 

    // what will work is getting the pixel coordiantes and converting them into uv coordinates

    // from UV coord <--- get the pixel coordinates
    // to UV coord <--- get the pixel coordinates

    // you have the record of all the coordinates

    public void setTimelineCoordinates()
    {

    }

    public void movePixelsDown(int x, int y, Color c)
    {
        texture2D.SetPixel(x, y, c);
    }

}