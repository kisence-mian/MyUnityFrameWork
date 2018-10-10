using UnityEngine;


    public static class CameraShakePresets
    {
        /// <summary>
        /// [One-Shot] A high magnitude, short, yet smooth shake.
        /// </summary>
        public static CameraShakeInstance Bump
        {
            get
            {
                CameraShakeInstance c = new CameraShakeInstance(2.5f, 4, 0.1f, 0.75f);
                c.PositionInfluence = Vector3.one * 0.15f;
                c.RotationInfluence = Vector3.one;
                return c;
            }
        }

        /// <summary>
        /// [One-Shot] An intense and rough shake.
        /// </summary>
        public static CameraShakeInstance Explosion
        {
            get
            {
                CameraShakeInstance c = new CameraShakeInstance(5f, 10, 0, 1.5f);
                c.PositionInfluence = Vector3.one * 0.25f;
                c.RotationInfluence = new Vector3(4, 1, 1);
                return c;
            }
        }

        /// <summary>
        /// [Sustained] A continuous, rough shake.
        /// </summary>
        public static CameraShakeInstance Earthquake
        {
            get
            {
                CameraShakeInstance c = new CameraShakeInstance(0.6f, 3.5f, 2f, 10f);
                c.PositionInfluence = Vector3.one * 0.25f;
                c.RotationInfluence = new Vector3(1, 1, 4);
                return c;
            }
        }

        /// <summary>
        /// [Sustained] A bizarre shake with a very high magnitude and low roughness.
        /// </summary>
        public static CameraShakeInstance BadTrip
        {
            get
            {
                CameraShakeInstance c = new CameraShakeInstance(10f, 0.15f, 5f, 10f);
                c.PositionInfluence = new Vector3(0, 0, 0.15f);
                c.RotationInfluence = new Vector3(2, 1, 4);
                return c;
            }
        }

        /// <summary>
        /// [Sustained] A subtle, slow shake. 
        /// </summary>
        public static CameraShakeInstance HandheldCamera
        {
            get
            {
                CameraShakeInstance c = new CameraShakeInstance(1f, 0.25f, 5f, 10f);
                c.PositionInfluence = Vector3.zero;
                c.RotationInfluence = new Vector3(1, 0.5f, 0.5f);
                return c;
            }
        }

        /// <summary>
        /// [Sustained] A very rough, yet low magnitude shake.
        /// </summary>
        public static CameraShakeInstance Vibration
        {
            get
            {
                CameraShakeInstance c = new CameraShakeInstance(0.4f, 20f, 2f, 2f);
                c.PositionInfluence = new Vector3(0, 0.15f, 0);
                c.RotationInfluence = new Vector3(1.25f, 0, 4);
                return c;
            }
        }

        /// <summary>
        /// [Sustained] A slightly rough, medium magnitude shake.
        /// </summary>
        public static CameraShakeInstance RoughDriving
        {
            get
            {
                CameraShakeInstance c = new CameraShakeInstance(1, 2f, 1f, 1f);
                c.PositionInfluence = Vector3.zero;
                c.RotationInfluence = Vector3.one;
                return c;
            }
        }
    }
