using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    [Tooltip("Prefab for the individual boids.")]
    [SerializeField] private GameObject _boidPrefab;

    [Tooltip("Number of boids along each axis in a cubic grid.")]
    [SerializeField] private int _boidsPerAxisRow = 5;

    [Tooltip("Distance at which the boids start avoiding each other.")]
    [SerializeField] private float _avoidanceRange = 8f;

    [Tooltip("Factor controlling avoidance behavior.")]
    [SerializeField] private float _avoidFactor = 0.5f;

    [Tooltip("Factor controlling matching velocity with neighboring boids.")]
    [SerializeField] private float _matchingFactor = 0.5f;

    [Tooltip("Factor controlling how quickly boids return to within the bounds.")]
    [SerializeField] private float _turnFactor = 10.0f;

    [Tooltip("Factor controlling how much the boids move towards the center of the flock.")]
    [SerializeField] private float _centeringFactor = 0.005f;

    [Tooltip("Maximum speed limit for the boids.")]
    [SerializeField] private float _maxSpeed = 10;

    [Tooltip("Minimum speed limit for the boids.")]
    [SerializeField] private float _minSpeed = 5;

    [Tooltip("Dimensions of the cubic bounding box for the flock.")]
    [SerializeField] private Vector3 _bounds = new Vector3(10, 10, 10);

    // List of boids in the flock.
    private List<Boid> _boids = new();

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

        // Calculate the initial positions for the boids in a cubic grid.
        Vector3 leftBackBottomCorner = transform.position - new Vector3(_bounds.x / 2, _bounds.y / 2, _bounds.z / 2);
        float offsetX = (_bounds.x - (_bounds.x * 0.2f)) / _boidsPerAxisRow;
        float offsetY = (_bounds.y - (_bounds.y * 0.2f)) / _boidsPerAxisRow;
        float offsetZ = (_bounds.z - (_bounds.z * 0.2f)) / _boidsPerAxisRow;
        float margin = (_bounds.x * 0.1f);

        // Instantiate the boids in the calculated positions.
        for (int z = 0; z < _boidsPerAxisRow; z++) {
            for (int y = 0; y < _boidsPerAxisRow; y++) {
                for (int x = 0; x < _boidsPerAxisRow; x++) {
                    var obj = Instantiate(_boidPrefab, leftBackBottomCorner + (2 * margin * Vector3.one) + new Vector3(x * offsetX, y * offsetY, z * offsetZ), Quaternion.identity);
                    _boids.Add(obj.GetComponent<Boid>());
                }
            }
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
        foreach (var boid in _boids) {
            // Initialize the variables for calculating the averages and close distances.
            float xpos_avg = 0, ypos_avg = 0, zpos_avg = 0, xvel_avg = 0, yvel_avg = 0, zvel_avg = 0, close_dx = 0, close_dy = 0, close_dz = 0;

            // Iterate through all the boids to calculate the averages and close distances.
            foreach (var otherboid in _boids) {

                // No reason to calculate agains itself.
                if (boid == otherboid)
                    continue;

                // Calculate the relative positions and distances.
                float dx = boid.transform.position.x - otherboid.transform.position.x;
                float dy = boid.transform.position.y - otherboid.transform.position.y;
                float dz = boid.transform.position.z - otherboid.transform.position.z;

                float squared_distance = dx * dx + dy * dy + dz * dz;

                // Check if the other boid is within the avoidance range.
                // If so, steer away from the other boid.
                if (squared_distance < _avoidanceRange) {
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
                       (xpos_avg - boid.transform.position.x) * _centeringFactor +
                       (xvel_avg - boid.velocity.x) * _matchingFactor) +
                       (close_dx * _avoidFactor) * Time.deltaTime;

            boid.velocity.y = (boid.velocity.y +
                       (ypos_avg - boid.transform.position.y) * _centeringFactor +
                       (yvel_avg - boid.velocity.y) * _matchingFactor) +
                       (close_dy * _avoidFactor) * Time.deltaTime;

            boid.velocity.z = (boid.velocity.z +
                       (zpos_avg - boid.transform.position.z) * _centeringFactor +
                       (zvel_avg - boid.velocity.z) * _matchingFactor) +
                       (close_dz * _avoidFactor) * Time.deltaTime;


            // Resteer the boid back if it leaves the boundries.
            if (boid.transform.position.x > (transform.position.x + _bounds.x / 2))
                boid.velocity.x -= _turnFactor * Time.deltaTime;
            if (boid.transform.position.x < (transform.position.x - _bounds.x / 2))
                boid.velocity.x += _turnFactor * Time.deltaTime;
            if (boid.transform.position.y > (transform.position.y + _bounds.y / 2))
                boid.velocity.y -= _turnFactor * Time.deltaTime;
            if (boid.transform.position.y < (transform.position.y - _bounds.y / 2))
                boid.velocity.y += _turnFactor * Time.deltaTime;
            if (boid.transform.position.z > (transform.position.z + _bounds.z / 2))
                boid.velocity.z -= _turnFactor * Time.deltaTime;
            if (boid.transform.position.z < (transform.position.z - _bounds.z / 2))
                boid.velocity.z += _turnFactor * Time.deltaTime;

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

        // Limit the amount of spheres there are displayed on the screen, because
        // the editor can't handle so many calls.
        int amountToSpawnPerRow = Mathf.Min(10, _boidsPerAxisRow);

        float margin = (_bounds.x * 0.1f);
        float offsetX = (_bounds.x - (margin * 2)) / amountToSpawnPerRow;
        float offsetY = (_bounds.y - (margin * 2)) / amountToSpawnPerRow;
        float offsetZ = (_bounds.z - (margin * 2)) / amountToSpawnPerRow;

        // Draw a grid of spheres that represent the spawn location of the instantiated boids.
        for (int z = 0; z < amountToSpawnPerRow; z++) {
            for (int y = 0; y < amountToSpawnPerRow; y++) {
                for (int x = 0; x < amountToSpawnPerRow; x++) {
                    Gizmos.color = (x * y * z) < 9 * 9 * 9 ? Color.yellow : Color.grey;
                    Gizmos.DrawWireSphere(leftBackBottomCorner + (1.5f * margin * Vector3.one) + new Vector3(x * offsetX, y * offsetY, z * offsetZ), 1);
                }
            }
        }
    }
}
