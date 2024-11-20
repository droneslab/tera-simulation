using UnityEngine;
using UnitySensors.Attribute;

namespace UnitySensors.Sensor.IMU
{
    public class IMUSensor : UnitySensor
    {
        private Transform _transform;

        [SerializeField, ReadOnly]
        private Vector3 _position;
        [SerializeField, ReadOnly]
        private Vector3 _velocity;
        [SerializeField, ReadOnly]
        private Vector3 _acceleration;
        [SerializeField, ReadOnly]
        private Quaternion _rotation;
        [SerializeField, ReadOnly]
        private Vector3 _RollPitchYaw;
        [SerializeField, ReadOnly]
        private Vector3 _angularVelocity;

        private Vector3 _position_tmp;
        private Vector3 _velocity_tmp;
        private Vector3 _acceleration_tmp;
        private Quaternion _rotation_tmp;
        private Vector3 _angularVelocity_tmp;
        private Vector3 _RollPitchYaw_tmp;

        private Vector3 _position_last;
        private Vector3 _velocity_last;
        private Quaternion _rotation_last;

        public Vector3 position { get => _position; }
        public Vector3 velocity { get => _velocity; }
        public Vector3 acceleration { get => _acceleration; }
        public Quaternion rotation { get => _rotation; }
        public Vector3 angularVelocity { get => _angularVelocity; }
        public Vector3 RollPitchYaw { get => _RollPitchYaw; }

        public Vector3 localVelocity { get => _transform.InverseTransformDirection(_velocity); }
        public Vector3 localAcceleration { get => _transform.InverseTransformDirection(_acceleration.normalized) * _acceleration.magnitude; }

        private Vector3 _gravityDirection;
        private float _gravityMagnitude;
        private float _time_last;
        public double mean;
        public double std_dev;

        protected override void Init()
        {
            _transform = this.transform;
            _gravityDirection = Physics.gravity.normalized;
            _gravityMagnitude = Physics.gravity.magnitude;
        }
        public void SetNoise(double mean, double std_dev)
        {
            Debug.Log($"Setting noise for {this.name} with mean={mean} and std_dev={std_dev}");
            this.mean = mean;
            this.std_dev = std_dev;
        }

        private void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;

            _position_tmp = _transform.position;
            _velocity_tmp = (_position_tmp - _position_last) / dt;
            _acceleration_tmp = (_velocity_tmp - _velocity_last) / dt;
            _acceleration_tmp -= _transform.InverseTransformDirection(_gravityDirection) * _gravityMagnitude;

            _rotation_tmp = _transform.rotation;
            Quaternion rotation_delta = Quaternion.Inverse(_rotation_last) * _rotation;
            rotation_delta.ToAngleAxis(out float angle, out Vector3 axis);
            float angularSpeed = (angle * Mathf.Deg2Rad) / dt;
            _angularVelocity_tmp = axis * angularSpeed;
            
            // Calculate Roll, Pitch, Yaw
            _RollPitchYaw_tmp.x = Mathf.PI + Mathf.Atan2(-_acceleration_tmp.y, Mathf.Sqrt(_acceleration_tmp.y * _acceleration_tmp.y + _acceleration_tmp.z * _acceleration_tmp.z)) * Mathf.Rad2Deg;
            _RollPitchYaw_tmp.y = Mathf.Atan2(-_acceleration_tmp.x, Mathf.Sqrt(_acceleration_tmp.y * _acceleration_tmp.y + _acceleration_tmp.z * _acceleration_tmp.z)) * Mathf.Rad2Deg;
            _RollPitchYaw_tmp.z = 0;

            _position_last = _position_tmp;
            _velocity_last = _velocity_tmp;
            _rotation_last = _rotation_tmp;
        }

        protected override void UpdateSensor()
        {
            _position = _position_tmp;
            _velocity = _velocity_tmp;
            _acceleration = _acceleration_tmp;

            _rotation = _rotation_tmp;
            _angularVelocity = _angularVelocity_tmp;
            _RollPitchYaw = _RollPitchYaw_tmp;

            if (onSensorUpdated != null)
                onSensorUpdated.Invoke();
        }

        protected override void OnSensorDestroy()
        {
        }
    }
}
