using System.Collections.Generic;
using UnityEngine;

namespace PSB.InGame
{
    public static class DebugModule
    {
        public static void PathVisualize(IEnumerable<Vector3> path)
        {
            foreach(var p in path)
            {
                var g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                var sp = p;
                sp.y = 1.0f;
                g.transform.position = sp;
            }
        }
    }
}
