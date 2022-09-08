using System;
using Terrain;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Terrain = Terrain.Terrain;
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
        private bool _hasSpawned;
        private global::Terrain.Terrain _terrain;

        void Start()
        {
            _rigidBody = GetComponent<Rigidbody>();
            _rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
            _camera = transform.Find("Camera");
            _terrain = terrainTransform.gameObject.GetComponent<global::Terrain.Terrain>();
            _world = _terrain.World;
            _collider = GetComponent<Collider>();
        }

        private bool IsGrounded()
        {
            const float castDistance = 0.1f;
            var bounds = _collider.bounds;
            var rayOrigin = new Vector3(bounds.center.x, bounds.min.y + castDistance/2,bounds.center.z);
            return Physics.Raycast(new Ray(rayOrigin, Vector3.down), out _, castDistance, PhysicsLayers.TerrainMask);
        }

        void Update()
        {
            if (!_hasSpawned)
            {
                if (_terrain.HasSpawn)
                {
                    _rigidBody.MovePosition(_terrain.Spawn);
                    _rigidBody.velocity = Vector3.zero;
                    _hasSpawned = true;
                }
            }
            
            var moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = Quaternion.Euler(new Vector3(0, _camera.rotation.eulerAngles.y, 0)) * moveDirection;
            moveDirection *= groundSpeed;
            moveDirection *= Time.deltaTime;
            moveDirection.y = _rigidBody.velocity.y;

            _rigidBody.velocity = moveDirection;

            if (Input.GetButtonDown("Jump") && IsGrounded())
            {
                _rigidBody.AddForce(new Vector3(0, jumpSpeed, 0));
            }
            
            if (Input.GetButtonDown("Dig"))
            {
                var rayStart = _camera.transform.position;
                var rayDirection = _camera.forward;
                const float rayDistance = float.PositiveInfinity;
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
                const float rayDistance = float.PositiveInfinity;
                var wasHit = Physics.Raycast(new Ray(rayStart, rayDirection), out var hit, rayDistance, PhysicsLayers.TerrainMask);
                if (!wasHit) return;
                
                var hitPoint = hit.point + rayDirection.normalized * 0.01f;
                var blockPosition = Vector3Int.FloorToInt(hitPoint);
                blockPosition += Vector3Int.FloorToInt(hit.normal.normalized);
                _world.SetBlock(blockPosition, Block.STONE);
            }
        }
    }
}