using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyDetection : MonoBehaviour, IEnemyComponent
{
    private EnemyComponentRefrences _enemyComponents;
    private Transform _detectedTarget;
    private CombatRules _combatRules;

    [SerializeField] private float _scanCooldown;       // Time between scans
    private float _scanTimeLeft;

    [SerializeField] private float _scanRadius;         // Detection radius
    [SerializeField] private float _sensingRadius;      // Max range for detection
    [SerializeField] private LayerMask _attackable;     // Layer mask to specify attackable targets
    [SerializeField] private LayerMask _canSee;         // Layer mask to check for visibility
    [SerializeField] private float _angleOfVision;      // Field of view for the detection
    [SerializeField] private float _eyeHeight;          // Height from which the enemy detects targets

    public Action<Transform> OnTargetDetected;
    public Action OnTargetLost;

    public void InitializeEnemyComponent(EnemyComponentRefrences enemyComponents)
    {
        _enemyComponents = enemyComponents;
        _combatRules = _enemyComponents.GetCombatRules;
        ResetDetection();
    }

    public void ResetDetection()
    {
        _detectedTarget = null;

        // Ensure EnemyUpdate is added only once
        if (_enemyComponents.OnUpdate != null && !_enemyComponents.OnUpdate.GetInvocationList().Contains((Action)EnemyUpdate))
        {
            _enemyComponents.OnUpdate += EnemyUpdate;
        }
        OnTargetLost?.Invoke();
    }

    private void EnemyUpdate()
    {
        DetectionRay(); // Visualize the detection rays
        DetectionLoop();
    }

    private void DetectionLoop()
    {
        if (_scanTimeLeft > 0)
        {
            _scanTimeLeft -= Time.deltaTime;
        }
        else
        {
            _scanTimeLeft = _scanCooldown;
            ScanForTarget(); // Start the scan for a target
        }
    }

    private void ScanForTarget()
    {
        // OverlapSphere checks for nearby colliders within the scan radius
        List<Collider> colliders = Physics.OverlapSphere(transform.position + Vector3.up * _eyeHeight, _scanRadius, _attackable).ToList();

        if (colliders == null || colliders.Count == 0) return;

        // Filter out targets not in front of the AI or beyond sensing radius
        colliders.RemoveAll(c =>
         !CheckIfInFront(c.transform.position) &&
         Vector3.Distance(transform.position, c.transform.position) > _sensingRadius
        );

        // Find the closest visible target
        while (colliders.Count > 0)
        {
            Collider closestCollider = ClosestColliderInList(colliders);

            // Cast a ray towards the closest target to check visibility
            RaycastHit hit;
            Vector3 eyePosition = transform.position + Vector3.up * _eyeHeight; // Adjusted eye position
            if (Physics.Raycast(eyePosition, closestCollider.transform.position - eyePosition, out hit, _scanRadius, _canSee))
            {
                if (hit.collider == closestCollider)
                {
                    CombatRules targetCombatRules = closestCollider.GetComponent<CombatRules>();
                    if (targetCombatRules != null && _combatRules.CanDamage(targetCombatRules, CombatRules.CombatMode.Team))
                    {
                        SetTarget(closestCollider.transform); // Target detected and can be damaged
                        return;
                    }
                }
            }
            colliders.Remove(closestCollider); // Remove the current collider and try the next one
        }
    }

    private bool CheckIfInFront(Vector3 pos)
    {
        // Calculate the angle between the enemy's forward direction and the position of the target
        float targetAngle = Mathf.Atan2(transform.position.z - pos.z, transform.position.x - pos.x) * Mathf.Rad2Deg + 90;
        float deltaAngleAIAndTarget = AngleDifference(targetAngle, -transform.eulerAngles.y);
        // Check if the target is within the field of view (angle of vision)
        return deltaAngleAIAndTarget < _angleOfVision / 2 && deltaAngleAIAndTarget > -_angleOfVision / 2;
    }

    public float AngleDifference(float angle1, float angle2)
    {
        float diff = (angle2 - angle1 + 180) % 360 - 180;
        return diff < -180 ? diff + 360 : diff;
    }

    public Collider ClosestColliderInList(List<Collider> colliders)
    {
        if (colliders == null || colliders.Count == 0)
            return null;

        float minDist = Mathf.Infinity;
        Collider closestCollider = null;

        foreach (Collider c in colliders)
        {
            float dist = Vector3.Distance(c.transform.position, transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closestCollider = c;
            }
        }
        return closestCollider;
    }

    private void SetTarget(Transform target)
    {
        _detectedTarget = target;
        _enemyComponents.OnUpdate -= EnemyUpdate;
        OnTargetDetected?.Invoke(target);
    }

    private void DetectionRay()
    {
        // Define the right and left "eyes" for the detection field
        Vector3 rightEye = new Vector3(Mathf.Sin(Mathf.Deg2Rad * _angleOfVision / 2), 0, Mathf.Cos(Mathf.Deg2Rad * _angleOfVision / 2));
        Vector3 leftEye = new Vector3(Mathf.Sin(Mathf.Deg2Rad * -_angleOfVision / 2), 0, Mathf.Cos(Mathf.Deg2Rad * -_angleOfVision / 2));

        // Draw debug rays in the editor for visualizing detection range and angle
        Debug.DrawRay(transform.position + Vector3.up * _eyeHeight, transform.rotation * rightEye * _scanRadius, Color.magenta);
        Debug.DrawRay(transform.position + Vector3.up * _eyeHeight, transform.rotation * leftEye * _scanRadius, Color.magenta);
    }

    public bool TargetIsVisible(Transform target, float maxDistance)
    {
        // Cast a ray to check if the target is visible
        RaycastHit hit;
        Vector3 eyePosition = transform.position + Vector3.up * _eyeHeight; // Adjusted eye position
        if (Physics.Raycast(eyePosition, target.position - eyePosition, out hit, maxDistance, _canSee))
        {
            return hit.collider.transform == target; // Ensure the hit object is the target
        }
        return false; // Target is not visible if ray does not hit
    }
}
