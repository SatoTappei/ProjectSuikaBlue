using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ˆêŽž’†’f

public class OnePlaySearchAI : MonoBehaviour
{
    static readonly int[,] EvaluateGrid =
    {
        { 30, -12, 0, -1, -1, 0, -12, 30 },
        { -12, -15, -3, -3, -3, -3, -15, -12 },
        { 0, -3, 0, -1, -1, 0, -3, 0 },
        { -1, -3, -1, -1, -1, -1, -3, -1 },
        { -1, -3, -1, -1, -1, -1, -3, -1 },        
        { 0, -3, 0, -1, -1, 0, -3, 0 },
        { -12, -15, -3, -3, -3, -3, -15, -12 },
        { 30, -12, 0, -1, -1, 0, -12, 30 },
    };
}
