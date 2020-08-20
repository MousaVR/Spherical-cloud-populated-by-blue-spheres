using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MarbleContainer : MonoBehaviour
{
    public MarbleBehavior MarblePrefab;

    private readonly KDTree<MarbleBehavior> _marbles = new KDTree<MarbleBehavior>();

    void Start()
    {
        StopAllCoroutines();
        for( var i = 0; i < 500; i++ )//500
        {
            GenerateMarble();
        }

        StartCoroutine( SpawnMarbles() );
    }

    IEnumerator SpawnMarbles()
    {
        while( true )
        {
            if( _marbles.Count < 1000 )//1000
            {
                for( var i = 0; i < 25; i++ )
                {
                   GenerateMarble();
                }
            }
            yield return new WaitForEndOfFrame();
            //yield return new WaitForSeconds( 0.5f );
        }
    }

    private void GenerateMarble()
    {
        var newMarble = Instantiate( MarblePrefab, new Vector3( Random.value, Random.value, Random.value ), Quaternion.identity );
        newMarble.transform.parent = this.transform;
        newMarble.transform.position = Random.insideUnitSphere * 100f;
        _marbles.Add( newMarble );
    }

    public void ClaimMarble( MarbleBehavior marble )
    {
        //if( _marbles.ContainsKey( marble.Id ) )
        //{
        //    _marbles.Remove( marble.Id );
        //}
        //implementing of object booling
        marble.WasClaimed = true;
        marble.transform.position = Random.insideUnitSphere * 100f;
        _marbles.UpdatePositions();
        marble.WasClaimed = false;
    }

    public MarbleBehavior GetCloseMarbleToPosition( Vector3 position )
    {
        return _marbles.FindClosest(position);
    }
}
