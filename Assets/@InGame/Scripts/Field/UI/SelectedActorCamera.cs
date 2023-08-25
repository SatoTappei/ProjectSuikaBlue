using UnityEngine;

public class SelectedActorCamera : ActorSelector
{
    [SerializeField] Transform _cameraRoot;

    protected override void OnSelected(GameObject actor)
    {
        _cameraRoot.position = actor.transform.position;
        _cameraRoot.SetParent(actor.transform);
    }
}
