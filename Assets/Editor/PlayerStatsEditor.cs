using System;
using UnityEditor;

[CustomEditor(typeof(PlayerStats))]
public class PlayerStatsEditor : Editor
{
    
    #region SerializedProperties
    private SerializedProperty playerLayer;
    private SerializedProperty camera;
    private SerializedProperty cameraRestrictionArea;
    
    private SerializedProperty maxSpeed;
    private SerializedProperty groundAcceleration;
    private SerializedProperty airAcceleration;
    private SerializedProperty groundDeceleration;
    private SerializedProperty airDeceleration;
    private SerializedProperty frictionAmount;
    
    private SerializedProperty doubleJump;
    private SerializedProperty jumpPower;
    private SerializedProperty jumpEndEarlyGravityModifier;
    private SerializedProperty coyoteTime;
    private SerializedProperty jumpBuffer;
    
    private SerializedProperty dash;
    private SerializedProperty dashKey;
    private SerializedProperty dashPower;
    private SerializedProperty dashTime;
    private SerializedProperty dashCooling;

    private SerializedProperty slide;
    private SerializedProperty slideFallSpeed;

    private SerializedProperty wallJump;
    private SerializedProperty wallJumpPower;
    private SerializedProperty wallJumpHorizontalPower;
    private SerializedProperty wallJumpTime;
    private SerializedProperty wallJumpGravityModifier;

    private SerializedProperty maxFallSpeed;
    private SerializedProperty fallAcceleration;

    private SerializedProperty groundingForce;

    private SerializedProperty collisionHorizontalDistance;
    private SerializedProperty collisionVerticalDistance;
    #endregion

    private void OnEnable()
    {
        playerLayer = serializedObject.FindProperty("playerLayer");
        camera = serializedObject.FindProperty("camera");
        cameraRestrictionArea = serializedObject.FindProperty("cameraRestrictionArea");
        
        maxSpeed = serializedObject.FindProperty("maxSpeed");
        groundAcceleration = serializedObject.FindProperty("groundAcceleration");
        airAcceleration = serializedObject.FindProperty("airAcceleration");
        groundDeceleration = serializedObject.FindProperty("groundDeceleration");
        airDeceleration = serializedObject.FindProperty("airDeceleration");
        frictionAmount = serializedObject.FindProperty("frictionAmount");
        
        doubleJump = serializedObject.FindProperty("doubleJump");
        jumpPower = serializedObject.FindProperty("jumpPower");
        jumpEndEarlyGravityModifier = serializedObject.FindProperty("jumpEndEarlyGravityModifier");
        coyoteTime = serializedObject.FindProperty("coyoteTime");
        jumpBuffer = serializedObject.FindProperty("jumpBuffer");
        
        dash = serializedObject.FindProperty("dash");
        dashKey = serializedObject.FindProperty("dashKey");
        dashPower = serializedObject.FindProperty("dashPower");
        dashTime = serializedObject.FindProperty("dashTime");
        dashCooling = serializedObject.FindProperty("dashCooling");
        
        slide = serializedObject.FindProperty("slide");
        slideFallSpeed = serializedObject.FindProperty("slideFallSpeed");
        
        wallJump = serializedObject.FindProperty("wallJump");
        wallJumpPower = serializedObject.FindProperty("wallJumpPower");
        wallJumpHorizontalPower = serializedObject.FindProperty("wallJumpHorizontalPower");
        wallJumpTime = serializedObject.FindProperty("wallJumpTime");
        wallJumpGravityModifier = serializedObject.FindProperty("wallJumpGravityModifier");
        
        maxFallSpeed = serializedObject.FindProperty("maxFallSpeed");
        fallAcceleration = serializedObject.FindProperty("fallAcceleration");
        groundingForce = serializedObject.FindProperty("groundingForce");
        
        collisionHorizontalDistance = serializedObject.FindProperty("collisionHorizontalDistance");
        collisionVerticalDistance = serializedObject.FindProperty("collisionVerticalDistance");
    }

    public override void OnInspectorGUI()
    {
        PlayerStats stats = (PlayerStats)target;
        
        serializedObject.Update();

        EditorGUILayout.PropertyField(playerLayer);
        EditorGUILayout.PropertyField(playerLayer);
        EditorGUILayout.PropertyField(camera);
        EditorGUILayout.PropertyField(cameraRestrictionArea);
        
        EditorGUILayout.PropertyField(maxSpeed);
        EditorGUILayout.PropertyField(groundAcceleration);
        EditorGUILayout.PropertyField(airAcceleration);
        EditorGUILayout.PropertyField(groundDeceleration);
        EditorGUILayout.PropertyField(airDeceleration);
        EditorGUILayout.PropertyField(frictionAmount);
        
        EditorGUILayout.PropertyField(doubleJump);
        EditorGUILayout.PropertyField(jumpPower);
        EditorGUILayout.PropertyField(jumpEndEarlyGravityModifier);
        EditorGUILayout.PropertyField(coyoteTime);
        EditorGUILayout.PropertyField(jumpBuffer);
        
        EditorGUILayout.PropertyField(dash);
        if (stats.dash)
        {
            EditorGUILayout.PropertyField(dashKey);
            EditorGUILayout.PropertyField(dashPower);
            EditorGUILayout.PropertyField(dashTime);
            EditorGUILayout.PropertyField(dashCooling);            
        }
        
        EditorGUILayout.PropertyField(slide);
        if (stats.slide)
        {
            EditorGUILayout.PropertyField(slideFallSpeed);
        }
        
        EditorGUILayout.PropertyField(wallJump);
        if (stats.wallJump)
        {
            EditorGUILayout.PropertyField(wallJumpPower);
            EditorGUILayout.PropertyField(wallJumpHorizontalPower);
            EditorGUILayout.PropertyField(wallJumpTime);
            EditorGUILayout.PropertyField(wallJumpGravityModifier);
        }
        
        EditorGUILayout.PropertyField(maxFallSpeed);
        EditorGUILayout.PropertyField(fallAcceleration);
        EditorGUILayout.PropertyField(groundingForce);
        
        EditorGUILayout.PropertyField(collisionHorizontalDistance);
        EditorGUILayout.PropertyField(collisionVerticalDistance);
        
        serializedObject.ApplyModifiedProperties();
    }
}
