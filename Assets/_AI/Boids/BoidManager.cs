using System;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    [SerializeField] private List<Boid> _boids = new();
    [SerializeField] private int _centerOfMassFraction = 100;
    [SerializeField] private int _offsetThreshold = 10;
    [SerializeField] private int _velocityFraction = 32;

    private List<Rigidbody> _boidRigidbodies = new();
    private Vector3 _centerOfMass;
    private Vector3 _offset;
    private Vector3 _velocity;

    private void Update()
    {
        UpdateBoids();
    }

    /// <summary>
    /// Calculates all the boid rules and applies them to the boid's velocity.
    /// </summary>
    private void UpdateBoids()
    {
        foreach (var boid in _boids) {
            _boidRigidbodies.Add(boid.GetComponent<Rigidbody>());

            _centerOfMass = CalculateDirection(boid);
            _offset = CalculateAvoidance(boid);
            _velocity = CalculateCohesion(boid);

            int i = _boids.IndexOf(boid);
            Rigidbody rb = _boidRigidbodies[i];

            rb.velocity += _centerOfMass + _offset + _velocity;
        }
    }

    /// <summary>
    ///  Calculate the percieved center of mass of the current boid 
    ///  based on the surrounding boids.
    /// </summary>
    private Vector3 CalculateDirection(Boid boid)
    {
        var centerOfMassTotal = Vector3.zero;
        foreach (var b in _boids) {
            centerOfMassTotal += b.transform.position;
        }

        var percievedCenter = (centerOfMassTotal - boid.transform.position) / (_boids.Count - 1);
        return (percievedCenter - boid.transform.position) / _centerOfMassFraction;
    }

    /// <summary>
    /// Calcuate the offset of the boids based on the serounding boids.
    /// </summary>
    /// <param name="boid"></param>
    private Vector3 CalculateAvoidance(Boid boid)
    {
        Vector3 c = Vector3.zero;
        foreach (var b in _boids) {
            if (b != boid) {
                if (Vector3.Distance(b.transform.position, boid.transform.position) < _offsetThreshold) {
                    c -= b.transform.position - boid.transform.position;
                }
            }
        }
        return c;
    }

    /// <summary>
    /// Calcuate the estimated velocity of all the boids and subtract a 
    /// small fraction of that to steer the boid away from the rest.
    /// <param name="boid"></param>
    private Vector3 CalculateCohesion(Boid boid)
    {
        Vector3 pvj = Vector3.zero;
        foreach (var b in _boids) {
            if (b != boid) {
                pvj += _boidRigidbodies[_boids.IndexOf(boid)].velocity;
            }
        }

        pvj /= _boids.Count - 1;

        return (pvj - _boidRigidbodies[_boids.IndexOf(boid)].velocity) / _velocityFraction;
    }
}
