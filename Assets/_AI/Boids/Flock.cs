using System;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField, Range(1, 100)] private float _seperationFactor = 5f;

    [Tooltip("Factor controlling matching velocity with neighboring boids.")]
    [SerializeField, Range(1, 100)] private float _alignmentFactor = 5f;

    [Tooltip("Factor controlling how much the boids move towards the center of the flock.")]
    [SerializeField, Range(1, 100)] private float _cohesionFactor = 5f;

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

    // Called every frame
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
            int y = (i / amountPerRow) % amountPerRow;
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
        var seperation = _seperationFactor / 100;
        var allignment = _alignmentFactor / 500;
        var cohesion = _cohesionFactor / 100;
        var turnSpeed = _turnFactor / 10;

        foreach (var boid in _boids) {
            // Initialize the variables for calculating the averages and close distances.
            float xpos_avg = 0, ypos_avg = 0, zpos_avg = 0, xvel_avg = 0, yvel_avg = 0, zvel_avg = 0, close_dx = 0, close_dy = 0, close_dz = 0;

            // Iterate through all the boids to calculate the averages and close distances.
            foreach (var otherboid in _boids) {

                // No reason to calculate against itself.
                if (boid == otherboid)
                    continue;

                // Calculate the relative positions and distances.
                float dx = boid.transform.position.x - otherboid.transform.position.x;
                float dy = boid.transform.position.y - otherboid.transform.position.y;
                float dz = boid.transform.position.z - otherboid.transform.position.z;

                float squared_distance = dx * dx + dy * dy + dz * dz;

                // Check if the other boid is within the avoidance range.
                // If so, steer away from the other boid.
                if (squared_distance < _seperationRange) {
                    close_dx += boid.transform.position.x - otherboid.transform.position.x;
                    close_dy += boid.transform.position.y - otherboid.transform.position.y;
                    close_dz += boid.transform.position.z - otherboid.transform.position.z;
                }

                // Add up all the position and velcoty values
                // of the other boids.
                xpos_avg += otherboid.transform.position.x;
                ypos_avg += otherboid.transform.position.y;
                zpos_avg += otherboid.transform.position.z;
                xvel_avg += otherboid.velocity.x;
                yvel_avg += otherboid.velocity.y;
                zvel_avg += otherboid.velocity.z;
            }
            // Calculate the average position and velocity.
            xpos_avg /= _boids.Count;
            ypos_avg /= _boids.Count;
            zpos_avg /= _boids.Count;
            xvel_avg /= _boids.Count;
            yvel_avg /= _boids.Count;
            zvel_avg /= _boids.Count;

            // Update the boid's velocity based on avoidance, cohesion, and alignment behaviors.
            boid.velocity.x = (boid.velocity.x +
                       (xpos_avg - boid.transform.position.x) * cohesion +
                       (xvel_avg - boid.velocity.x) * allignment) +
                       (close_dx * seperation) * Time.deltaTime;

            boid.velocity.y = (boid.velocity.y +
                       (ypos_avg - boid.transform.position.y) * cohesion +
                       (yvel_avg - boid.velocity.y) * allignment) +
                       (close_dy * seperation) * Time.deltaTime;

            boid.velocity.z = (boid.velocity.z +
                       (zpos_avg - boid.transform.position.z) * cohesion +
                       (zvel_avg - boid.velocity.z) * allignment) +
                       (close_dz * seperation) * Time.deltaTime;


            // Resteer the boid back if it leaves the boundries.
            if (boid.transform.position.x > (transform.position.x + _bounds.x / 2))
                boid.velocity.x -= turnSpeed * Time.deltaTime;
            if (boid.transform.position.x < (transform.position.x - _bounds.x / 2))
                boid.velocity.x += turnSpeed * Time.deltaTime;
            if (boid.transform.position.y > (transform.position.y + _bounds.y / 2))
                boid.velocity.y -= turnSpeed * Time.deltaTime;
            if (boid.transform.position.y < (transform.position.y - _bounds.y / 2))
                boid.velocity.y += turnSpeed * Time.deltaTime;
            if (boid.transform.position.z > (transform.position.z + _bounds.z / 2))
                boid.velocity.z -= turnSpeed * Time.deltaTime;
            if (boid.transform.position.z < (transform.position.z - _bounds.z / 2))
                boid.velocity.z += turnSpeed * Time.deltaTime;

            // Limit boid speed within defined limits.
            float speed = boid.velocity.sqrMagnitude;

            if (speed < _minSpeed * _minSpeed) {
                boid.velocity.x = (boid.velocity.x / speed) * _minSpeed;
                boid.velocity.y = (boid.velocity.y / speed) * _minSpeed;
                boid.velocity.z = (boid.velocity.z / speed) * _minSpeed;
            }
            if (speed > _maxSpeed * _maxSpeed) {
                boid.velocity.x = (boid.velocity.x / speed) * _maxSpeed;
                boid.velocity.y = (boid.velocity.y / speed) * _maxSpeed;
                boid.velocity.z = (boid.velocity.z / speed) * _maxSpeed;
            }

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
            int y = (i / amountPerRow) % amountPerRow;
            int z = i / (amountPerRow * amountPerRow);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(leftBackBottomCorner + offset / 2 + Vector3.Scale(offset, new Vector3(x, z, y)), 1);
        }
    }
}
