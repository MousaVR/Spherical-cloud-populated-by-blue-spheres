using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class MarbleContainer : MonoBehaviour
{
    public MarbleBehavior MarblePrefab;
    private readonly KDTree<MarbleBehavior> _marbles = new KDTree<MarbleBehavior>();
    [HideInInspector]
    public static MarbleContainer instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    void Start()
    {      
        for( var i = 0; i < 500; i++ )
        {
            GenerateMarble();
        }

        StartCoroutine( SpawnMarbles() );
    }

    IEnumerator SpawnMarbles()
    {
        if( _marbles.Count < 1000 )
        {
            for( var i = 0; i < 25; i++ )
            {
                GenerateMarble();
            }
        yield return new WaitForEndOfFrame();
        }     
    }

    private void GenerateMarble()
    {
        var newMarble = Instantiate( MarblePrefab, new Vector3( Random.value, Random.value, Random.value ), Quaternion.identity );
        newMarble.transform.parent = this.transform;
        newMarble.transform.position = Random.insideUnitSphere * 100f;
        _marbles.Add( newMarble );
    }

    public void UpdateMarbelsPositions()
    {
        _marbles.UpdatePositions();
    }

    public MarbleBehavior GetCloseMarbleToPosition( Vector3 position )
    {
        return _marbles.FindClosest(position);
    }
}
