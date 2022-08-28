using System;
using Terrain;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Player
{
    public class Player : MonoBehaviour
    {
        public float groundSpeed = 20.0f;
        public float jumpSpeed = 20.0f;
        public Transform terrainTransform;

        private World _world;
        private Rigidbody _rigidBody;
        private Transform _camera;
        private Collider _collider;
        private bool? _grounded;

        void Start()
        {
            _rigidBody = GetComponent<Rigidbody>();
            _rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
            _camera = transform.Find("Camera");
            var terrain = terrainTransform.gameObject.GetComponent<Terrain.Terrain>();
            _rigidBody.MovePosition(terrain.Spawn);
            _world = terrain.World;
            _collider = GetComponent<CapsuleCollider>();
        }

        private void FixedUpdate()
        {
            _grounded = null;
            if (Input.GetButtonDown("Jump") && IsGrounded())
            {
                _rigidBody.AddForce(new Vector3(0, jumpSpeed, 0));
            }
        }

        private bool IsGrounded()
        {
            if (_grounded.HasValue)
            {
                return _grounded.Value;
            }
        
            const float castDistance = 0.1f;
            var rayOrigin = new Vector3(_collider.bounds.center.x, _collider.bounds.min.y + castDistance/2,_collider.bounds.center.z);
            var wasHit = Physics.Raycast(new Ray(rayOrigin, Vector3.down), out _, castDistance, PhysicsLayers.TerrainMask);
            _grounded = wasHit;
            return wasHit;
        }


        void Update()
        {
            var moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = Quaternion.Euler(new Vector3(0, _camera.rotation.eulerAngles.y, 0)) * moveDirection;
            moveDirection *= groundSpeed;
            moveDirection *= Time.deltaTime;
            moveDirection.y = _rigidBody.velocity.y;

            _rigidBody.velocity = moveDirection;

            if (Input.GetButtonDown("Dig"))
            {
                var rayStart = _camera.transform.position;
                var rayDirection = _camera.forward;
                var rayDistance = float.PositiveInfinity;
                var wasHit = Physics.Raycast(new Ray(rayStart, rayDirection), out var hit, rayDistance, PhysicsLayers.TerrainMask);
                if (wasHit)
                {
                    var hitPoint = hit.point + rayDirection.normalized * 0.01f;
                    var blockPosition = new Vector3Int((int) Math.Floor(hitPoint.x), (int) Math.Floor(hitPoint.y),
                        (int) Math.Floor(hitPoint.z));
                    _world.SetBlock(blockPosition, Block.AIR);
                }
            }

            if (Input.GetButtonDown("Place"))
            {
                var rayStart = _camera.transform.position;
                var rayDirection = _camera.forward;
                var rayDistance = float.PositiveInfinity;
                var wasHit = Physics.Raycast(new Ray(rayStart, rayDirection), out var hit, rayDistance, PhysicsLayers.TerrainMask);
                if (wasHit)
                {
                    var hitPoint = hit.point + rayDirection.normalized * 0.01f;
                    var blockPosition = Vector3Int.FloorToInt(hitPoint);
                    blockPosition += Vector3Int.FloorToInt(hit.normal.normalized);
                    _world.SetBlock(blockPosition, Block.STONE);
                }
            }
        }
    }
}