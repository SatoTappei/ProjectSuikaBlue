using UnityEngine;

namespace PSB.InGame
{
    public class GameManager : MonoBehaviour
    {
        void Awake()
        {
            FieldClickHandler.OnFieldClicked += M;
        }

        void OnDestroy()
        {
            FieldClickHandler.OnFieldClicked -= M;
        }

        void M(Cell cell)
        {
            Debug.Log(cell.ResourceType);
        }
    }
}
