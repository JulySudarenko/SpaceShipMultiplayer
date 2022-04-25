using System;
using Mechanics;
using UnityEngine;

namespace Editor
{
    [Serializable]
    public sealed class PlanetCharacteristics
    {
        public PlanetOrbit Planet;
        public Vector3 Position;
        public float Speed;
    }
    
    [Serializable]
    public sealed class CrystalZoneCharacteristics
    {
        public Vector3 ZonePosition;
        public Vector3 ZoneRadius;
    }
}
