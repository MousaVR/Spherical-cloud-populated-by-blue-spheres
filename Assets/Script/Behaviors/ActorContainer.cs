using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MarbleContainer))]
public class ActorContainer : MonoBehaviour
{
    public ActorBehavior ActorPrefab;
    private readonly List<ActorBehavior> _actors = new List<ActorBehavior>();

    void Start()
    {
        for( var i = 0; i < 1000; i++ )
        {
            var newActor = Instantiate( ActorPrefab, this.transform );
            newActor.transform.position = Random.insideUnitSphere * 100f;
            _actors.Add( newActor );
        }
    }
}
