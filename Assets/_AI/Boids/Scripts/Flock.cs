using System;
using System.Collections.Generic;
using UnityEngine;

namespace KMGDEVAI.Boids
{
    public class Flock : MonoBehaviour
    {
        [Header("Flock Settings")]
        [Tooltip("Prefab for the individual boids.")]
        [SerializeField] private GameObject _boidPrefab = null;

        [Tooltip("Number of boids along each axis in a cubic grid.")]
        [SerializeField, Min(0)] private int _boidsAmount = 5;

        [Header("Flock Rules")]
        [Tooltip("Distance at which the boids start avoiding each other.")]
        [SerializeField, Min(0)] private float _seperationRange = 8f;

        [Tooltip("Factor controlling avoidance behavior.")]
        [SerializeField, Min(1)] private float _seperationFactor = 5f;

        [Tooltip("Factor controlling matching velocity with neighboring boids.")]
        [SerializeField, Min(1)] private float _alignmentFactor = 5f;

        [Tooltip("Factor controlling how much the boids move towards the center of the flock.")]
        [SerializeField, Min(1)] private float _cohesionFactor = 5f;

        [Header("Bounds")]
        [Tooltip("Factor controlling how quickly boids return to within the bounds.")]
        [SerializeField, Min(1)] private float _turnFactor = 1.0f;

        [Tooltip("Maximum speed limit for the boids.")]
        [SerializeField, Min(1)] private float _maxSpeed = 20;

        [Tooltip("Minimum speed limit for the boids.")]
        [SerializeField, Min(1)] private float _minSpeed = 10;

        [Tooltip("Dimensions of the cubic bounding box for the flock.")]
        [SerializeField, Min(1)] private Vector3 _bounds = new(10, 10, 10);

        // List of boids in the flock.
        private readonly List<Boid> _boids = new();

        private void Start()
        {
            // Instantiate the boids and initialize their velocity.
            InstantiateBoids();
            InitializeBoids();
        }

        private void Update()
        {
            // Update the behavior of the flock.
            UpdateBoids();
        }

        // Instantiate the boids in a cubic grid formation within the defined flocking boundaries.
        private void InstantiateBoids()
        {
            if (!_boidPrefab)
                return;

            // Get the corner of the cube to draw the spheres in a grid from that point.
            Vector3 leftBackBottomCorner = transform.position - _bounds / 2;

            int amountPerRow = (int)Math.Ceiling(Math.Pow(_boidsAmount, 1.0 / 3.0));
            Vector3 offset = _bounds / amountPerRow;

            // Instantiate all the boids in a grid, seperated and centered.
            for (int i = 0; i < _boidsAmount; i++) {
                int x = i % amountPerRow;
                int y = i / amountPerRow % amountPerRow;
                int z = i / (amountPerRow * amountPerRow);

                var obj = Instantiate(_boidPrefab, leftBackBottomCorner + offset / 2 + Vector3.Scale(offset, new Vector3(x, z, y)), Quaternion.identity);
                _boids.Add(obj.GetComponent<Boid>());
            }
        }

        // Initialize the velocity of each boid to the average speed between the specified minimum and maximum speeds.
        private void InitializeBoids()
        {
            foreach (var boid in _boids) {
                boid.velocity = Vector3.one * ((_minSpeed + _maxSpeed) / 2);
            }
        }

        // Update the position and velocity of each boid based on avoidance, cohesion, and alignment behaviors.
        private void UpdateBoids()
        {
            float separation = _seperationFactor / 1000.0f;
            float alignment = _alignmentFactor / 1000.0f;
            float cohesion = _cohesionFactor / 10000.0f;
            float turnSpeed = _turnFactor / 100.0f;
            Vector2 speedLimit = new(_minSpeed / 100, _maxSpeed / 100);

            foreach (var boid in _boids) {
                Vector3 posAvg = Vector3.zero;
                Vector3 velAvg = Vector3.zero;
                Vector3 closeDelta = Vector3.zero;

                // Iterate through all the boids to calculate the averages and close distances.
                foreach (var otherboid in _boids) {

                    // No reason to calculate against itself.
                    if (boid == otherboid)
                        continue;

                    // Calculate the relative positions and distances.
                    Vector3 relativePositions = boid.transform.position - otherboid.transform.position;

                    float squared_distance = relativePositions.sqrMagnitude;

                    // Check if the other boid is within the avoidance range.
                    // If so, steer away from the other boid.
                    if (squared_distance < _seperationRange * _seperationRange) {
                        closeDelta += boid.transform.position - otherboid.transform.position;
                    }

                    // Add up all the position and velcoty values
                    // of the other boids.
                    posAvg += otherboid.transform.position;
                    velAvg += otherboid.velocity;
                }
                // Calculate the average position and velocity.
                posAvg /= _boids.Count;
                velAvg /= _boids.Count;

                // Update the boid's velocity based on avoidance, cohesion, and alignment behaviors.
                boid.velocity = boid.velocity +
                                (posAvg - boid.transform.position) * cohesion +
                                (velAvg - boid.velocity) * alignment +
                                closeDelta * separation *
                                Time.deltaTime;

                Vector3 offsets = _bounds / 2;

                // Resteer the boid back if it leaves the boundries.
                if (boid.transform.position.x > transform.position.x + offsets.x)
                    boid.velocity.x -= turnSpeed * Time.deltaTime;
                if (boid.transform.position.x < transform.position.x - offsets.x)
                    boid.velocity.x += turnSpeed * Time.deltaTime;
                if (boid.transform.position.y > transform.position.y + offsets.y)
                    boid.velocity.y -= turnSpeed * Time.deltaTime;
                if (boid.transform.position.y < transform.position.y - offsets.y)
                    boid.velocity.y += turnSpeed * Time.deltaTime;
                if (boid.transform.position.z > transform.position.z + offsets.z)
                    boid.velocity.z -= turnSpeed * Time.deltaTime;
                if (boid.transform.position.z < transform.position.z - offsets.z)
                    boid.velocity.z += turnSpeed * Time.deltaTime;

                // Limit the boid's speed within defined limits.
                float speed = boid.velocity.magnitude;
                boid.velocity = boid.velocity.normalized * Mathf.Clamp(speed, speedLimit.x, speedLimit.y);

                // Update boid position based on its velocity
                boid.transform.position += boid.velocity;
            }
        }

        private void OnDrawGizmos()
        {
            if (!enabled)
                return;

            // Draw the boundaries of the flocking area as a green wire cube.
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, new Vector3(_bounds.x, _bounds.y, _bounds.z));

            // Draw a wire sphere on the positions where a boid will be instantiated.
            if (Application.isPlaying)
                return;

            // Get the corner of the cube to draw the spheres in a grid from that point.
            Vector3 leftBackBottomCorner = transform.position - _bounds / 2;

            int amountPerRow = (int)Math.Ceiling(Math.Pow(_boidsAmount, 1.0 / 3.0));
            Vector3 offset = _bounds / amountPerRow;

            // Draw a grid of spheres that represent the spawn location of the instantiated boids.
            for (int i = 0; i < _boidsAmount; i++) {
                int x = i % amountPerRow;
                int y = i / amountPerRow % amountPerRow;
                int z = i / (amountPerRow * amountPerRow);

                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(leftBackBottomCorner + offset / 2 + Vector3.Scale(offset, new Vector3(x, z, y)), 1);
            }
        }
    }
}