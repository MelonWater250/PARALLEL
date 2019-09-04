using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// プレイヤー操作
    /// </summary>
    public class PlayerController : PlayerBase
    {
        //入力キー
        private string MOVEAXIS_X = "Axis_X_";
        private string MOVEAXIS_Y = "Axis_Y_";
        private string ROTATEAXIS_Y = "Rotation_Y_";

        //入力キー更新
        private void UpdateInputKey()
        {
            _playerNum = GetComponentInParent<Planet.PlanetManager>().PlayerNum;
            MOVEAXIS_X += _playerNum.ToString();
            MOVEAXIS_Y += _playerNum.ToString();
            ROTATEAXIS_Y += _playerNum.ToString();
        }
        
        protected override void Start()
        {
            base.Start();
            UpdateInputKey();
        }

        private void Update()
        {
            if (GameManager.Instance.IsGame() == false) return;
            if (_planetManager.IsAlive() == false) return;
            if (CanMove == false) return;
            
            InputKey();
            MovePlayer(moveAxis, rotateAxis);
        }

        /// <summary>
        /// キー入力
        /// </summary>
        private void InputKey()
        {
            moveAxis = new Vector2(Input.GetAxis(MOVEAXIS_X), Input.GetAxis(MOVEAXIS_Y));
            rotateAxis = Input.GetAxis(ROTATEAXIS_Y);
        }

        //移動
        protected override void MovePlayer(Vector2 moveAxis, float rotateAxis = 0)
        {
            base.MovePlayer(moveAxis, rotateAxis);
        }

        //ライトの接触判定
        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
        }

        //本体の接触判定
        protected override void OnCollisionEnter(Collision collision)
        {
            base.OnCollisionEnter(collision);
        }
    }
}