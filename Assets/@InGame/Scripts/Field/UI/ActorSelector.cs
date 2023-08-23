using UnityEngine;

public class ActorSelector : MonoBehaviour
{
    const int RayDistance = 100; // “K“–‚È’l

    [SerializeField] LayerMask _actorLayer;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsRaycastHitActor(out GameObject actor))
            {
                OnSelected(actor);
            }
            else
            {
                OnDeselected();
            }
        }

        OnUpdate();
    }

    protected virtual void OnUpdate() { }

    protected virtual void OnSelected(GameObject actor) { }
    protected virtual void OnDeselected() { }

    bool IsRaycastHitActor(out GameObject actor)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, RayDistance, _actorLayer))
        {
            actor = hit.collider.gameObject;
            return true;
        }
        else
        {
            actor = null;
            return false;
        }
    }
}
