using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeizerPathGroup : MonoBehaviour
{
    public BeizerPathData beizerPathData;
    public GameObject controlPointPrefab;
    public GameObject intermediateControlPointPrefab; 
    public GameObject intermediatePointPrefab; 

    public void Awake()
    {
        beizerPathData = ScriptableObject.CreateInstance<BeizerPathData>();
        beizerPathData.beizerPathGroup = this;
        beizerPathData.beizerPoints = new List<BeizerPoint>();

        beizerPathData.curveXbeizer = new AnimationCurve();
        beizerPathData.curveYbeizer = new AnimationCurve();
        beizerPathData.curveZbeizer = new AnimationCurve();


    }

    public void ResetBeizerPathCurves()
    {
        beizerPathData.beizerPoints = new List<BeizerPoint>();

        beizerPathData.curveXbeizer = new AnimationCurve();
        beizerPathData.curveYbeizer = new AnimationCurve();
        beizerPathData.curveZbeizer = new AnimationCurve();
    }
}
