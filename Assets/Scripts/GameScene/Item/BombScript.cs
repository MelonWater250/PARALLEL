using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace Item
{
    public class BombScript : MonoBehaviour
    {
        //動作中か
        [HideInInspector]
        public bool IsActive = false;

        //移動中か
        private bool _isMove = false;

        //停止
        public void StopMove() { _isMove = false; }

        //カウントダウン中か
        public bool IsCountdown { set; get; } = false;

        //爆発済か
        private bool _isExploded = false;
        public bool IsExploded() { return _isExploded; }

        //移動可能か
        public bool CanMove() { return (IsActive == true)  && (_isExploded == false); }

        //アイテムの固有ID
        [HideInInspector]
        public int ItemID;

        //攻撃者のプレイヤー番号(スポーン時に書き換え)
        private int _attackPlayerNum = 1;
        public void SetPlayerNum(int playerNum) { _attackPlayerNum = playerNum; }

        public bool IsMine(int playerNum) { return playerNum == _attackPlayerNum ? true : false; }

        [Header("メインパラメーター")]
        [SerializeField, Tooltip("アイテム拡縮にかかる時間")]
        private float _scallingTime = 0.5f;

        [SerializeField, Tooltip("攻撃力")]
        private float _power = 10.0f;

        [SerializeField, Tooltip("移動速度")]
        private float _speed = 5.0f;

        [SerializeField, Tooltip("カウントダウン時間")]
        private float _lifeTime = 10.0f;

        [Header("爆発")]
        //初期スケール値
        private Vector3 _defaultScale = new Vector3(1, 1, 1);

        [SerializeField, Tooltip("カウントダウン0の時のスケール")]
        private Vector3 _bombScale = new Vector3(2, 2, 2);

        //初期カラー
        private Color _defaultColor = new Color(20, 20, 20);

        [SerializeField, Tooltip("カウントダウン0の時のカラー")]
        private Color _bombColor = new Color(240, 20, 20);

        [SerializeField, Tooltip("本体のマテリアル")]
        private MeshRenderer _bodyMeshRenderer;

        [Header("Effect")]
        [SerializeField, Tooltip("爆発パーティクル")]
        private ParticleSystem _explosionParticle = null;

        [SerializeField, Tooltip("爆発パーティクルの生成数")]
        private int _explosionParticleEmitCount = 30;

        [SerializeField, Tooltip("爆発にかかる時間")]
        private float _explosionTime = 3.0f;

        //重力加速度
        private float _localGravity = 9.8f;

        private Rigidbody _rb;

        public void Init(int itemID)
        {
            ItemID = itemID;
            IsActive = true;
            _isMove = false;
            _isExploded = false;
            _rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            IsActive = true;
            _isMove = false;
            _isExploded = false;
            _defaultScale = transform.localScale;
            _defaultColor = _bodyMeshRenderer.material.color;
            _localGravity = Physics.gravity.magnitude;
            _rb = GetComponent<Rigidbody>();

            StartCoroutine(CountDown());
        }

        //爆発のカウントダウン
        private IEnumerator CountDown()
        {
            float time = _lifeTime;
            while (time > 0)
            {
                //カウントダウン中なら処理
                if (IsCountdown == false)
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
            StopMove();
            transform.parent = null;
            StartCoroutine(MoveItemCol(targetObj/* ,(Vector3)offset ,accelerator*/));
        }

        //private IEnumerator MoveItemCol(GameObject targetObj, Vector3 offsetPos, float accelerator = 1.0f)
        //{
        //    if (GameManager.Instance.IsGame() == false) yield break;

        //    float newSpeed = _speed * accelerator;
        //    _isMove = true;
        //    while (CanMove() && GameManager.Instance.IsGame())
        //    {
        //        transform.position += ((targetObj.transform.position + targetObj.transform.TransformVector(offsetPos)) - transform.position).normalized * newSpeed * Time.deltaTime;
        //        yield return null;
        //    }
        //}
        private IEnumerator MoveItemCol(GameObject targetObj)
        {
            _isMove = true;
            while (CanMove() && GameManager.Instance.IsGame() && _isMove == true)
            {
                Vector3 targetPos = targetObj.transform.position - transform.position;
                _rb.AddForce(targetPos.normalized * _localGravity, ForceMode.Acceleration);
                Debug.Log(_rb.velocity);
                yield return new WaitForFixedUpdate();
                //yield return null;
            }
        }

        /// <summary>
        /// 拡大
        /// </summary>
        public void ExpandingItem(float? time = null, Action OnCompleteAction = null)
        {
            if (time == null) time = _scallingTime;
            transform.DOScale(_defaultScale, (float)time).OnComplete(() => OnCompleteAction());
        }

        /// <summary>
        /// 縮小
        /// </summary>
        public void ShrinkItem(float? time = null, Action OnCompleteAction = null)
        {
            if (time == null) time = _scallingTime;
            transform.DOScale(Vector3.zero, (float)time).OnComplete(() => OnCompleteAction());
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
                planetManager.TakeDamage(_power);
            }

            //縮小
            ShrinkItem(_explosionTime);
            //爆発パーティクル生成
            _explosionParticle.Emit(_explosionParticleEmitCount);
            //パーティクル生成中は処理停止
            yield return new WaitWhile(_explosionParticle.IsAlive);
            IsActive = false;
            gameObject.SetActive(false);
            yield return null;
        }

        private void OnTriggerStay(Collider other)
        {
            //爆発済ならリターン
            if (_isExploded == true) return;

            if (other.tag == TagContainer.PLANET_TAG)
            {
                
                //StartCoroutine(ExplosionItemCol());
                //PlanetManager planet = other.GetComponent<PlanetManager>();

                ////相手の惑星なら
                //if (IsMine(planet.PlayerNum) == false)
                //{
                //    planet.TakeDamage(_power);
                //    StartCoroutine(ExplosionItemCol());
                //}
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.tag == TagContainer.PLANET_TAG)
            {
                //カウントダウン開始
                IsCountdown = true;
                StopMove();
            }
        }
    }
}