using System.Collections.Generic;

namespace HotfixBusiness.Entity
{
    public class SphereCharacterPlayerData : EntityData
    {
        //Range(0, 100)
        public float MaxSpeed { get; set; } = 10f;
        public float MaxClimbSpeed { get; set; } = 4f;
        public float MaxSwimSpeed { get; set; } = 5f;

        //Range(0, 100)
        public float MaxAcceleration { get; set; } = 10f;
        public float MaxAirAcceleration { get; set; } = 1f;
        public float MaxClimbAcceleration { get; set; } = 40f;
        public float MaxSwimAcceleration { get; set; } = 5f;

        //Range(0, 10)
        public float JumpHeight { get; set; } = 2f;
        //Range(0, 5)
        public int MaxAirJumps { get; set; } = 0;

        //Range(0, 90)
        public float MaxGroundAngle { get; set; } = 25f;
        //Range(0, 90)
        public float MaxStairsAngle { get; set; } = 50f;
        //Range(90, 170)
        public float MaxClimbAngle { get; set; } = 140f;

        //Min(0f)
        public float MaxSnapSpeed { get; set; } = 100f;
        public float ProbeDistance { get; set; } = 1f;
        public float SubmergenceOffset { get; set; } = 0.5f;
        //Min(0.1f)
        public float SubmergenceRange { get; set; } = 1f;
        //Min(0f)
        public float Buoyancy { get; set; } = 1f;
        //Range(0f, 10f)
        public float WaterDrag { get; set; } = 1f;
        //Range(0.01f, 1f)
        public float SwimThreshold { get; set; } = 0.5f;

        //Min(0.1f)
        public float BallRadius { get; set; } = 0.5f;
        //Min(0f)
        public float BallAlignSpeed { get; set; } = 180f;
        //Min(0f)
        public float BallAirRotation { get; set; } = 0.5f;
        //Min(0f)
        public float BallSwimRotation { get; set; } = 2f;

        public float Hp { get; set; } = 5;


        public SphereCharacterPlayerData(int entityId, int typeId,string groupName, string assetName) : base(entityId, typeId, groupName,assetName)
        {

        }
    }
}