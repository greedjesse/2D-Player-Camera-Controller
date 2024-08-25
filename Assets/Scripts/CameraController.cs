using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    
    [SerializeField] private float moveThreshold = 0.05f;
    [SerializeField] private float shiftAmount = 1;
    [SerializeField] private float speedFactor = 100;
    
    private float startZ;

    void Start()
    {
        startZ = transform.position.z;
        
        // Setup restrictions.
        _restrictionCols = new List<Collider2D>();
        _restrictionTrans = new List<Transform>();
        
        foreach (var restriction in restrictions) 
        {
            _restrictionCols.Add(restriction.GetComponent<Collider2D>());
            _restrictionTrans.Add(restriction.transform.GetChild(0).transform);
        }
    }
    
    void Update()
    {
        HandleRestrictions();
        HandleMovement();
    }

    #region Movement

    [HideInInspector] public Vector3 targetPosition;
    private Vector3 _finalPosition;
    
    private void HandleMovement()
    {
        if (_useRestrictedPos) _finalPosition = _restrictedPos;
        else _finalPosition = targetPosition;

        if (Vector3.Distance(_finalPosition, transform.position) > moveThreshold) 
            transform.position += (_finalPosition - transform.position) / speedFactor;
    }

    #endregion

    #region Restrictions

    [SerializeField] private List<GameObject> restrictions;
    private List<Collider2D> _restrictionCols;
    private List<Transform> _restrictionTrans;
    
    private bool _useRestrictedPos;
    private Vector3 _restrictedPos;

    private void HandleRestrictions()
    {
        targetPosition = player.position + new Vector3(shiftAmount * ((int)player.transform.localScale.x == 1 ? 1 : -1), 0, startZ);
        
        _useRestrictedPos = false;
        for (int i = 0; i < restrictions.Count; i++)
        {
            Collider2D col = _restrictionCols[i];
            if (!col.OverlapPoint(targetPosition)) continue;
            _restrictedPos = _restrictionTrans[i].position;
            _useRestrictedPos = true;
            break;
        }
    }
    
    #endregion
}
