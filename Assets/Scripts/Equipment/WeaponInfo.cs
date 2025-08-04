using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInfo : MonoBehaviour
{
    public List<Transform> muzzlePoints; // eŒûi•¡”‘Î‰j
    public List<Transform> cartridgePoints;//–òä°”ro
    public List<Transform> backBrlastPoints;//–òä°”ro
    public mountPointType point;
    public enum mountPointType
    {
        None, Hand, Shoulder
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
