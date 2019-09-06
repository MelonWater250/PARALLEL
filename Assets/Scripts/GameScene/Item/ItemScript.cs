﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace Item
{
    /// <summary>
    /// アイテム個別の処理
    /// アクティブ状態管理、接触処理、移動・拡縮
    /// </summary>
    public class ItemScript : MonoBehaviour
    {
        //動作中か
        [HideInInspector]
        public bool IsActive { set; get; } = false;

        //移動中か
        private bool _isMove = false;

        //移動停止
        public void StopMove()
        {
            if (MoveCol != null) { StopCoroutine(MoveCol); MoveCol = null; }
            _rb.velocity = Vector3.zero;
            _isMove = false;
        }

        //カウントダウン中か
        private bool _isCountdown = false;
        public bool IsCountDown() { return _isCountdown; }

        //爆発済か
        private bool _isExploded = false;
        public bool IsExploded() { return _isExploded; }

        //移動可能か
        public bool CanMove() { return (IsActive == true)  && (_isExploded == false); }

        //アイテムの固有ID
        [HideInInspector]
        public int ItemID;

        //攻撃者のプレイヤー番号(スポーン時に書き換え)
        [SerializeField]
        private int _attackPlayerNum = 1;
        public void SetPlayerNum(int playerNum) { _attackPlayerNum = playerNum; }

        public bool IsMine(int playerNum) { return playerNum == _attackPlayerNum ? true : false; }

        [Header("メインパラメーター")]
        [SerializeField,Tooltip("移動速度の倍率(Default:1.0)")]
        private float _speed = 1.0f;

        [SerializeField, Tooltip("攻撃力")]
        private float _power = 10.0f;

        [SerializeField, Tooltip("アイテム拡縮にかかる時間")]
        private float _scallingTime = 0.5f;


        [SerializeField, Tooltip("カウントダウン時間")]
        private float _lifeTime = 10.0f;

        [Header("爆発")]
        //初期スケール値
        private Vector3 _defaultScale = new Vector3(1, 1, 1);

        [SerializeField, Tooltip("カウント0の時のスケール")]
        private Vector3 _bombScale = new Vector3(2, 2, 2);

        //初期カラー
        private Color _defaultColor = new Color(20, 20, 20);

        [SerializeField, Tooltip("カウントダウン0の時のカラー")]
        private Color _bombColor = new Color(240, 20, 20);

        [SerializeField, Tooltip("本体のマテリアル")]
        private MeshRenderer _bodyMeshRenderer = null;

        [Header("Effect")]
        [SerializeField, Tooltip("爆発パーティクル")]
        private ParticleSystem _explosionParticle = null;

        [SerializeField, Tooltip("爆発パーティクルの生成数")]
        private int _explosionParticleEmitCount = 30;

        [SerializeField, Tooltip("爆発にかかる時間")]
        private float _explosionTime = 3.0f;

        [SerializeField, Tooltip("移動中のパーティクル")]
        private ParticleSystem _movingParticle = null;

        //加速度
        private float _defaultAcceleration = 9.8f;

        private Rigidbody _rb;

        //移動コルーチン保持用
        private Coroutine MoveCol = null;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="itemID">固有ID</param>
        public void Init(int itemID)
        {
            ItemID = itemID;
            IsActive = true;
            _isMove = false;
            _isExploded = false;
            _rb = GetComponent<Rigidbody>();

            //移動中のエフェクト停止
            _movingParticle.Stop();
        }

        private void Start()
        {
            IsActive = true;
            _isMove = false;
            _isExploded = false;
            _defaultScale = transform.localScale;
            _defaultColor = _bodyMeshRenderer.material.color;
            _defaultAcceleration = Physics.gravity.magnitude;
            _rb = GetComponent<Rigidbody>();

            //移動中のエフェクト停止
            _movingParticle.Stop();

            StartCoroutine(CountDown());
        }
        //爆発のカウントダウン
        private IEnumerator CountDown()
        {
            float time = _lifeTime;
            while (time > 0)
            {
                //カウントダウン中なら処理
                if (_isCountdown == true)
                {
                    //スケール拡大
                    transform.localScale = Vector3.Lerp(_bombScale, _defaultScale, time / _lifeTime);
                    //カラー変更
                    _bodyMeshRenderer.material.color = Color.Lerp(_bombColor, _defaultColor, time / _lifeTime);

                    time -= Time.deltaTime;
                }
                yield return null;
            }
            StartCoroutine(ExplosionItemCol());
        }

        /// <summary>
        /// 移動
        /// </summary>
        /// <param name="targetObj">移動先</param>
        /// <param name="offset">移動先のピボットの相対位置</param>
        /// <param name="accelerator">速度の倍率</param>
        public void MoveItem(GameObject targetObj, Vector3? offset = null, float accelerator = 1.0f)
        {
            if (IsActive == false) return;
            if (targetObj == null) return;
            if (offset == null) offset = Vector3.zero;
            //移動停止
            StopMove();
            //カウントダウン停止
            _isCountdown = false;
            transform.parent = null;
            MoveCol = StartCoroutine(MoveItemCol(targetObj ,(Vector3)offset ,accelerator));
        }
        private IEnumerator MoveItemCol(GameObject targetObj, Vector3 offsetPos, float accelerator)
        {
            _isMove = true;

            //移動中のエフェクト開始
            _movingParticle.Play();

            while (CanMove() && GameManager.Instance.IsGame() && _isMove == true)
            {
                //Debug.Log(name + targetObj.name);
                Vector3 targetDirection = targetObj.transform.position - transform.position + offsetPos;
                _rb.AddForce(targetDirection.normalized * _defaultAcceleration * _speed * accelerator, ForceMode.Acceleration);
                yield return new WaitForFixedUpdate();
                //yield return null;
            }

            //移動中のエフェクト停止
            _movingParticle.Stop();
        }

        /// <summary>
        /// 停止&爆発&非表示
        /// </summary>
        public IEnumerator ExplosionItemCol(Planet.PlanetManager planetManager = null)
        {
            _isExploded = true;
            StopMove();

            if(planetManager != null)
            {
                //planetManager.TakeDamage(_power);
            }

            //縮小
            transform.DOScale(Vector3.zero,_scallingTime);

            //爆発パーティクル生成
            _explosionParticle.Emit(_explosionParticleEmitCount);
            //パーティクル生成中は処理停止
            yield return new WaitWhile(_explosionParticle.IsAlive);
            IsActive = false;
            gameObject.SetActive(false);
            yield return null;
        }

        //private void OnTriggerStay(Collider other)
        //{
        //    //爆発済ならリターン
        //    if (_isExploded == true) return;

        //    if (other.tag == TagContainer.PLANET_TAG)
        //    {
                
        //        //StartCoroutine(ExplosionItemCol());
        //        //PlanetManager planet = other.GetComponent<PlanetManager>();

        //        ////相手の惑星なら
        //        //if (IsMine(planet.PlayerNum) == false)
        //        //{
        //        //    planet.TakeDamage(_power);
        //        //    StartCoroutine(ExplosionItemCol());
        //        //}
        //    }
        //}

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.tag == TagContainer.PLANET_TAG)
            {
                //カウントダウン再開
                _isCountdown = true;
                StopMove();
            }
        }
    }
}