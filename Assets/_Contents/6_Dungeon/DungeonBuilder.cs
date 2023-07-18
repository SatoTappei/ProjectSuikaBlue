using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonBuilder : MonoBehaviour
{
    [SerializeField] TextAsset _dungeonBlueprint;

    void Start()
    {
        string[] lines = _dungeonBlueprint.text.Split("\n");

    }

    void Update()
    {
        
    }

    void Generate(string[] lines)
    {

    }
}
