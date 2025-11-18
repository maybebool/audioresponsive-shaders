using UnityEngine;

namespace Flocking {
    /// <summary>
    /// Represents the conduct values associated with a boid in a flocking simulation.
    /// </summary>
    /// <remarks>
    /// The BoidConductValues struct encapsulates data for the position, forward direction,
    /// ray steering vector, and steering factor of an individual boid.
    /// It also defines the size of its structure in memory.
    /// </remarks>
    public struct BoidConductValues {
        public Vector3 Position { get; set; }
        public Vector3 Forward { get; set; }
        public Vector3 RaySteer { get; set; }
        public float Steering { get; set; }
        public static int Size => (sizeof(float) * 3 * 3) + sizeof(float);
    }
}