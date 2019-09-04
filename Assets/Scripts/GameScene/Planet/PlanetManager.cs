using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Item;

namespace Planet
{
    /// <summary>
    /// 惑星上のプレイヤーの管理、自転、相手の攻撃(アイテムに当たったときの判定)
    /// </summary>
    public class PlanetManager : MonoBehaviour
    {
        public int PlayerNum = 1; 

        //生きているか
        [HideInInspector]
        private bool _isAlive = true;
        public bool IsAlive()
        {
            if (_nowHealth <= 0) _isAlive = false;
            return _isAlive;
        }
    
        [Header("パラメーター")]
        [SerializeField,Tooltip("惑星の初期体力")]
        private float _maxHealth = 100;
        //惑星の体力
        private float _nowHealth;

        [SerializeField, Tooltip("自転速度")]
        private float _rotateSpeed = 10.0f;



        void Awake()
        {
            _nowHealth = _maxHealth;
        }

        void Update()
        {
            if (IsAlive() == false) return;

            RotatePlanet();
        }
        
        /// <summary>
        /// 自転
        /// </summary>
        private void RotatePlanet()
        {
            transform.Rotate(0, _rotateSpeed * Time.deltaTime, 0);
        }
        
        /// <summary>
        /// 攻撃を受けたときのダメージ計算
        /// </summary>
        /// <param name="damage">攻撃者の攻撃力</param>
        public void TakeDamage(float damage = 0)
        {
            //体力が0以下ならリターン
            if (IsAlive() == false) { return; }

            //体力が0を下回るなら体力を0にする
            _nowHealth = Mathf.Max(0, _nowHealth - damage);
            
            //カメラを揺らす


            //体力が0以下になったなら死亡
            if (IsAlive() == false) { ExplodePlanet(); }
        }

        /// <summary>
        /// 惑星爆発
        /// </summary>
        private void ExplodePlanet()
        {
            Debug.Log("Dead");
        }
    }
}