using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Planet;

namespace Player
{
    public abstract class PlayerBase : MonoBehaviour
    {
        //プレイヤーの番号
        [HideInInspector]
        protected int _playerNum = 1;

        [Header("パラメーター")]
        [SerializeField,Tooltip("移動速度")]
        protected float _moveSpeed = 90.0f;

        [SerializeField, Tooltip("回転速度")]
        protected float _rotateSpeed = 90.0f;

        //移動回転方向
        protected Vector2 moveAxis;
        protected float rotateAxis;

        protected PlanetManager _planetManager;

        protected virtual void Start()
        {
            _planetManager = GetComponentInParent<PlanetManager>();
        }

        /// <summary>
        /// 移動
        /// </summary>
        /// <param name="moveAxis">移動方向</param>
        /// <param name="rotateAxis">回転方向(右回転：+)</param>
        protected virtual void MovePlayer(Vector2 moveAxis, float rotateAxis = 0)
        {
            //前後移動(X軸回転)
            transform.RotateAround(_planetManager.transform.position,                     //回転軸(惑星)
                _planetManager.transform.rotation * transform.localRotation * new Vector3(moveAxis.normalized.y, 0, 0),//回転方向(プレイヤーのローカル回転×入力方向)
                _moveSpeed * Time.deltaTime);   //回転量(入力量×スピード)
            

            //左右移動(Z軸回転)
            transform.RotateAround(_planetManager.transform.position,                     //回転軸(惑星)
                _planetManager.transform.rotation * transform.localRotation * new Vector3(0, 0, moveAxis.normalized.x), //回転方向(プレイヤーのローカル回転×入力方向)
                _moveSpeed * Time.deltaTime);  //回転量(入力量×スピード)

            ////移動方向に傾ける
            //_ufoObj.transform.localRotation = Quaternion.Euler(Input_YAxis * _maxRotateAngle,
            //    0, Input_XAxis * _maxRotateAngle);


            //左右回転(Y軸)
            transform.Rotate(new Vector3(0, rotateAxis * _rotateSpeed * Time.deltaTime, 0));
        }
    }
}