using System.Collections.Generic;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody _rigidBody;

    public float speed = 20.0f;
    public float jumpSpeed = 20.0f;
    public Transform terrainTransform;
    
    private Vector3 _moveDirection = Vector3.zero;
    private Transform _camera;
    private Collider _collider;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        _camera = transform.Find("Camera");
        var spawn = terrainTransform.gameObject.GetComponent<Terrain.Terrain>().Spawn;
        _rigidBody.MovePosition(spawn);
        _collider = GetComponent<BoxCollider>();
    }

    private void FixedUpdate()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            _rigidBody.AddForce(new Vector3(0, jumpSpeed, 0));
        }
    }

    private bool IsGrounded()
    {
        var castDistance = 0.1f;
        var rayOrigin = new Vector3(_collider.bounds.center.x, _collider.bounds.min.y + castDistance/2,_collider.bounds.center.z);
        return Physics.Raycast(new Ray(rayOrigin, Vector3.down), out _, castDistance, PhysicsLayers.TerrainMask);
    }


    void Update()
    {
        _moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        _moveDirection *= speed;
        _moveDirection = Quaternion.Euler(new Vector3(0, _camera.rotation.eulerAngles.y, 0)) * _moveDirection;
        
        _rigidBody.velocity =_moveDirection * Time.deltaTime;
    }
}