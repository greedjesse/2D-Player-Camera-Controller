using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class PlayerStats : ScriptableObject
{
    [Header("Layer")] 
    public LayerMask playerLayer;
    public LayerMask camera;
    public LayerMask cameraRestrictionArea;
    
    
    [Header("Move")]
    public float maxSpeed = 13;
    public float groundAcceleration = 120;
    public float airAcceleration = 25;
    public float groundDeceleration = 15;
    public float airDeceleration = 15;
    public float frictionAmount = 0.5f;

    [Header("Jump")] 
    public bool doubleJump = true;
    public float jumpPower = 36;
    [Range(1f, 10f)]public float jumpEndEarlyGravityModifier = 3;
    [Range(0.0f, 0.5f)] public float coyoteTime = 0.15f;
    [Range(0.0f, 0.5f)] public float jumpBuffer = 0.2f;
    
    [Header("Dash")] 
    public bool dash = true;
    public KeyCode dashKey = KeyCode.C;
    public float dashPower = 40;
    [Range(0.05f, 0.5f)] public float dashTime = 0.1f;
    public float dashCooling = 0.4f;

    [Header("Slide")] 
    public bool slide = true;
    public float slideFallSpeed = 10;

    [Header("Wall Jump")] 
    public bool wallJump = true;
    public float wallJumpPower = 36;
    public float wallJumpHorizontalPower = 24;
    [Range(0.0f, 1.0f)] public float wallJumpTime = 0.1f;
    [Range(0.1f, 1f)] public float wallJumpGravityModifier = 0.75f;
    
    [Header("Fall")]
    public float maxFallSpeed = 40;
    public float fallAcceleration = 100;
    
    [Header("Grounding")]
    [Range(-0.01f, -10f)] public float groundingForce = -5f;
    
    [Header("Collision")]
    [Range(0.01f, 1f)] public float collisionHorizontalDistance = 0.05f;
    [Range(0.01f, 1f)] public float collisionVerticalDistance = 0.5f;
}
