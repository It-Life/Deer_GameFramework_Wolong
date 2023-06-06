using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minimalist.Bar.SampleScene
{
    public class RotatingCameraBhv : MonoBehaviour
    {
        public float rotationSpeed = 10;

        private Transform _transform;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
        }

        private void Update()
        {
            _transform.RotateAround(Vector3.zero, Vector3.up, Time.deltaTime * rotationSpeed);
        }
    }
}