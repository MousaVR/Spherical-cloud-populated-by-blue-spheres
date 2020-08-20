using UnityEngine;

public class ActorBehavior : MonoBehaviour
{
    private enum State
    {
        Idle,
        Hunting,
    }

    private State _currentState;
    private MarbleBehavior _currentTarget;

    void Start()
    {
        _currentState = State.Idle;
    }

    void Update()
    {
        switch( _currentState )
        {
            case State.Idle:
                UpdateIdle();
                break;
            case State.Hunting:
                UpdateMoving();
                break;
        }
    }

    private void UpdateIdle()
    {
        _currentTarget = MarbleContainer.instance.GetCloseMarbleToPosition( this.transform.position );
        if( _currentTarget != null )
        {
            _currentState = State.Hunting;
        }
    }

    private void UpdateMoving()
    {
        if( _currentTarget.WasClaimed )
        {
            _currentTarget = null;
            _currentState = State.Idle;
            return;
        }

        var thisToTarget = _currentTarget.transform.position - this.transform.position;
        var thisToTargetDirection = thisToTarget.normalized;
        this.transform.position += thisToTargetDirection *10* Time.deltaTime;

        if( thisToTarget.magnitude < 0.1f )
        {
            _currentTarget.Claim();
            _currentTarget = null;
            _currentState = State.Idle;
        }
    }
}
