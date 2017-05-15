using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColor : MonoBehaviour {

    private static List<Color> list = new List<Color> { Color.yellow, Color.gray, Color.green, Color.black, Color.blue, Color.cyan, Color.magenta, Color.red };
    private Material mat;

    void Awake()
    {
        mat = GetComponent<MeshRenderer>().materials[0];
        Debug.Log(mat);
        Color color = list[Random.Range(0, list.Count)];
        list.Remove(color);
        mat.color = color;
    }

    void Start()
    {

    }


}
