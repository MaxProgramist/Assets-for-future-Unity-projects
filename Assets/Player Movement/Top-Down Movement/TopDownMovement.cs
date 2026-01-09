using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TopDownMovement : MonoBehaviour
{
    public enum MovementType
    {
        StepByStep,
        Continuously
    }
    enum MovementDirections
    {
        None,
        FourDirection,
        EightDirection,
        AnyDirection
    }


    [SerializeField] private MovementType movementType;

    [Header("Movement")]
    [SerializeField] private float movementMaxSpeed;
    [SerializeField] private float movementAcceleration;
    [SerializeField] private float movementDeceleration;
    [Space(8)]
    [SerializeField] private MovementDirections movementDirections;

    [Header("Movement")]
    [SerializeField] private float lenghtOfStep;
    [SerializeField] private float timeBtwSteps;
    [Space(8)]
    [SerializeField] private AnimationCurve movementAnimationCurve;
    [SerializeField] private float movementAnimationDuration;
    [Space(5)]
    [SerializeField] private LayerMask wallLayer;

    [Header("Technical stuff")]
    [SerializeField] private Rigidbody2D rigidbody;

    Vector2 moveInput;
    Vector2 currentVelocity;

    float currentTimeBtwSteps;


    void OnValidate()
    {
        if (movementType == MovementType.StepByStep)
        {
            movementDirections = MovementDirections.None;
            movementAnimationDuration = Mathf.Min(movementAnimationDuration, timeBtwSteps);
        }
    }

    void Awake()
    {
        rigidbody.gravityScale = 0f;
    }
    
    void Update()
    {
        Vector2 rawInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        moveInput = GetFilteredDirection(rawInput);
    }

    void FixedUpdate()
    {
        if (movementType == MovementType.Continuously) ContinuouslyMovement();
        else StepByStepMovement();
    }

    private void StepByStepMovement()
    {
        if (currentTimeBtwSteps > timeBtwSteps && CanMove(moveInput, lenghtOfStep))
        {
            StartCoroutine(MoveBetweenPoints(transform.position, transform.position + (Vector3)moveInput * lenghtOfStep));
            currentTimeBtwSteps = 0;
        } else
        {
            currentTimeBtwSteps += Time.deltaTime;
        }
    }

    private void ContinuouslyMovement()
    {
        Vector2 targetVelocity = moveInput * movementMaxSpeed;

        float currentAcceleration = (moveInput.magnitude > 0.01) ? movementAcceleration : movementDeceleration;

        currentVelocity = Vector2.MoveTowards(rigidbody.velocity, targetVelocity, currentAcceleration * Time.fixedDeltaTime);

        rigidbody.velocity = currentVelocity;
    }

    private Vector2 GetFilteredDirection(Vector2 rawInput)
    {
        if (rawInput.sqrMagnitude < 0.01f)
            return Vector2.zero;

        Vector2 normalizedInput = new Vector2(Mathf.Abs(rawInput.x) > 0.01f ? Mathf.Sign(rawInput.x) : 0f, Mathf.Abs(rawInput.y) > 0.01f ? Mathf.Sign(rawInput.y) : 0f);

        if (movementDirections == MovementDirections.FourDirection || movementType == MovementType.StepByStep)
            if (Mathf.Abs(normalizedInput.x) > Mathf.Abs(normalizedInput.y))
                return new Vector2(Mathf.Sign(normalizedInput.x), 0f);
            else
                return new Vector2(0f, Mathf.Sign(normalizedInput.y));
        else if (movementDirections == MovementDirections.EightDirection)
            return new Vector2(normalizedInput.x, normalizedInput.y);
        else if(movementDirections == MovementDirections.AnyDirection)
            return normalizedInput;

        return Vector2.zero;
    }

    public bool CanMove(Vector2 direction, float distance)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, wallLayer);
        return !hit;
    }

    IEnumerator MoveBetweenPoints(Vector2 startPoint, Vector2 endPoint)
    {
        float elapsedTime = 0f;

        while (elapsedTime < movementAnimationDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / movementAnimationDuration);
            float curveT = movementAnimationCurve.Evaluate(t);

            transform.position = Vector2.LerpUnclamped(startPoint, endPoint, curveT);

            yield return null;
        }

        transform.position = endPoint;
    }

    private void OnDrawGizmos()
    {
        if (movementType == MovementType.Continuously) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)moveInput * lenghtOfStep);
    }
}
