using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorInfo : MonoBehaviour
{
    public GameObject armature;
    public List<Transform> nozzle;
    public List<Transform> accessories;
    public GameObject booster;
    [Header("Backpack")]
    public Transform ShoulderR;
    public Transform ShoulderL;
}