using UnityEngine;

namespace Planet
{
    public class CannonLine : MonoBehaviour
    {
        [SerializeField, Tooltip("回転速度")]
        private float _rotateSpeed = -90.0f;

        void Update()
        {
            transform.Rotate(0, _rotateSpeed * Time.deltaTime, 0);
        }
    }
}