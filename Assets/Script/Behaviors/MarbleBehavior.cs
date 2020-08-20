using System.Collections;
using UnityEngine;

public class MarbleBehavior : MonoBehaviour
{
    private bool _wasClaimed;
    private MeshRenderer _rendrer;
    public bool WasClaimed
    {
        get
        {
            return _wasClaimed;
        }
        set
        {
            if( !_wasClaimed && value )
            {
                StartCoroutine( DisplayScore() );
            }
            _wasClaimed = value;
        }
    }
    public float Value { get; private set; }

    private Transform _textboxContainer;
    private TextMesh _textmesh;

    void Start()
    {
        _textmesh = this.transform.Find( "TextboxContainer/Textbox/ScoreText" ).gameObject.GetComponent<TextMesh>();
        _textboxContainer = this.transform.GetChild(0);
        _rendrer = gameObject.GetComponent<MeshRenderer>();
        _textboxContainer.gameObject.SetActive( false );
        WasClaimed = false;
    }

    private IEnumerator DisplayScore()
    {
        var steps = 60;
        _textboxContainer.localScale = Vector3.zero;
        Value = UnityEngine.Random.value * 100f - 25f;
        _textmesh.text = Value.ToString("##.#");
        _textboxContainer.gameObject.SetActive( true );
        for( var i = 0; i < steps; i++ )
        {
            _textboxContainer.localScale += Vector3.one / steps;
            yield return new WaitForEndOfFrame();
        }
        //the use of object booling
        _textboxContainer.gameObject.SetActive(false);
        Reposition();
    }
    
    //instead of destroying the marable we will reposition it
    private void Reposition()
    {
        _rendrer.enabled = true;
        transform.position = UnityEngine.Random.insideUnitSphere * 100f;
        MarbleContainer.instance.UpdateMarbelsPositions();
        WasClaimed = false;
    }

    public void Claim()
    {
        WasClaimed = true;
        _rendrer.enabled = false;
    }

}
