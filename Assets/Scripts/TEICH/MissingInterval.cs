using UnityEngine;
public class MissingInterval
{
    public int trackNumber;
    public float startT;
    public float endT;
    public GameObject overlayTop;
    public GameObject overlayBottom;
    public GameObject overlayWater; //collider object for correction

    public MissingInterval(int i, float s, float e, GameObject top, GameObject bottom, GameObject water)
    {
        trackNumber = i;
        startT = s;
        endT = e;
        overlayTop = top;
        overlayBottom = bottom;
        overlayWater = water;
    }
}


