using System.Collections.Generic;
using UnityEngine;

namespace PSB.InGame
{
    public class FieldManager : MonoBehaviour
    {
        [SerializeField] FieldBuilder _fieldBuilder;

        Cell[,] _field;
        Bresenham _bresenham;

        public Cell[,] Field
        {
            get
            {
                _field ??= _fieldBuilder.Build();
                return _field;
            }
        }

        void Start()
        {
            // Ç∆ÇËÇ†Ç¶Ç∏ê∂ê¨ÇµÇƒÇ®Ç≠
            _field = _fieldBuilder.Build();
            _bresenham = new(_field);
        }



        //public void 

        //List<GameObject> _objList = new();
        //void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.Space)) M();
        //}

        //void M()
        //{
        //    if (_objList.Count > 0)
        //    {
        //        foreach (GameObject obj in _objList)
        //        {
        //            obj.SetActive(false);
        //        }

        //        _objList.Clear();
        //    }

        //    int sx = Random.Range(0, _field.GetLength(1));
        //    int sy = Random.Range(0, _field.GetLength(0));
        //    int gx = Random.Range(0, _field.GetLength(1));
        //    int gy = Random.Range(0, _field.GetLength(0));
            
        //    GameObject go1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    go1.name = "Start";
        //    go1.transform.position = _field[sy, sx].Pos + Vector3.up;
        //    _objList.Add(go1);
        //    GameObject go2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    go2.name = "Goal";
        //    go2.transform.position = _field[gy, gx].Pos + Vector3.up;
        //    _objList.Add(go2);

        //    _bresenham.TryGetPath(new Vector2Int(sx, sy), new Vector2Int(gx, gy), out Stack<Vector2Int> path);
        //    foreach (Vector2Int p in path)
        //    {
        //        GameObject go3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //        go3.transform.position = _field[p.y, p.x].Pos + Vector3.up;
        //        _objList.Add(go3);
        //    }
        //}
    }
}