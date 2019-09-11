using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Planet;

namespace Item
{
    /// <summary>
    /// アイテム個別の処理
    /// アクティブ状態管理、接触処理、移動・拡縮
    /// </summary>
    public class ItemScript : MonoBehaviour
    {
        //動作中か
        //[HideInInspector]
        public bool IsActive = false;

        //移動中か
        private bool _isMove = false;

        //移動停止
        public void StopMove()
        {
            if (MoveCol != null) { StopCoroutine(MoveCol); MoveCol = null; }
            _rb.velocity = Vector3.zero;
            _isMove = false;
            //移動中のエフェクト停止
            _movingParticle.Stop();
        }

        //カウントダウン中か
        [HideInInspector]
        public bool IsCountdown = false;

        //爆発済か
        private bool _isExploded = false;
        public bool IsExploded() { return _isExploded; }

        //移動可能か
        public bool CanMove() { return (IsActive == true)  && (_isExploded == false); }

        //アイテムの固有ID
        [HideInInspector]
        public int ItemID { get; set; }

        //攻撃者のプレイヤー番号(スポーン時に書き換え)
        [SerializeField]
        private int _attackPlayerNum = 1;
        public void SetPlayerNum(int playerNum) { _attackPlayerNum = playerNum; }

        public bool IsMine(int playerNum) { return playerNum == _attackPlayerNum ? true : false; }

        [Header("メインパラメーター")]
        [SerializeField,Tooltip("移動速度の倍率(Default:1.0)")]
        private float _speed = 1.0f;

        //加速度
        private float _defaultAcceleration = 9.8f;

        [SerializeField, Tooltip("攻撃力")]
        private int _power = 10;

        [SerializeField, Tooltip("アイテム拡縮にかかる時間")]
        private float _scallingTime = 0.5f;


        [SerializeField, Tooltip("カウントダウン時間")]
        private float _lifeTime = 10.0f;

        //経過時間保持用
        private float _time = 0;


        [Header("爆発")]
        //初期スケール値
        private Vector3 _defaultScale = new Vector3(1, 1, 1);

        [SerializeField, Tooltip("カウント0の時のスケール")]
        private Vector3 _itemScale = new Vector3(2, 2, 2);

        //初期カラー
        [SerializeField]
        private Color _defaultColor = new Color(0.14f,0.14f,0.14f);

        [SerializeField, Tooltip("カウントダウン0の時のカラー")]
        private Color _itemColor = new Color(0.95f,0.15f,0.15f);

        [SerializeField, Tooltip("本体のマテリアル")]
        private MeshRenderer _bodyMeshRenderer = null;

        [SerializeField, Tooltip("爆発の影響半径")]
        private float _explosionRadius = 5.0f;

        [Header("Effect")]
        [SerializeField, Tooltip("爆発パーティクル")]
        private ParticleSystem _explosionParticle = null;

        [SerializeField, Tooltip("爆発パーティクルの生成数")]
        private int _explosionParticleEmitCount = 30;

        [SerializeField, Tooltip("移動中のパーティクル")]
        private ParticleSystem _movingParticle = null;
        
        private Rigidbody _rb;
        
        //移動コルーチン保持用
        private Coroutine MoveCol = null;

        /// <summary>
        /// 初期化
        /// </summary>
        public void Init()
        {
            StopMove();
            IsActive = false;
            _isExploded = false;
            IsCountdown = false;
            transform.localScale = _defaultScale;
            _bodyMeshRenderer.material.color = _defaultColor;
            _time = 0;
            //移動中のエフェクト停止
            _movingParticle.Stop();
        }
        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="itemID">固有ID</param>
        public void Init(int itemID)
        {
            StopMove();
            ItemID = itemID;
            IsActive = false;
            _isExploded = false;
            IsCountdown = false;
            transform.localScale = _defaultScale;
            _bodyMeshRenderer.material.color = _defaultColor;
            _time = 0;
            //移動中のエフェクト停止
            _movingParticle.Stop();
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            StopMove();
            IsActive = false;
            _isMove = false;
            _isExploded = false;
            IsCountdown = false;
            _defaultScale = transform.localScale;
            _defaultColor = _bodyMeshRenderer.material.color;
            _defaultAcceleration = Physics.gravity.magnitude;
            _time = 0;

            //移動中のエフェクト停止
            _movingParticle.loop = false;
            _movingParticle.Stop();
        }

        //爆発のカウントダウン
        private void Update()
        {
            //if (GameManager.Instance.IsGame() == false) return; 
            if (IsActive == false) return;
            if (_isExploded == true) return;
            if (IsCountdown == false) return;

            _time += Time.deltaTime;
            //スケール拡大
            transform.localScale = Vector3.Lerp(_defaultScale, _itemScale, _time / _lifeTime);
            //カラー変更
            _bodyMeshRenderer.material.color = Color.Lerp(_defaultColor, _itemColor, _time / _lifeTime);

            //寿命を超えたら爆発
            if (IsActive && _time >= _lifeTime) StartCoroutine(ExplosionItemCol(true));
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
            IsCountdown = false;
            transform.parent = null;
            MoveCol = StartCoroutine(MoveItemCol(targetObj ,(Vector3)offset ,accelerator));
        }
        private IEnumerator MoveItemCol(GameObject targetObj, Vector3 offsetPos, float accelerator)
        {
            _isMove = true;

            //移動中のエフェクト開始
            _movingParticle.loop = true;
            _movingParticle.Play();

            while (CanMove() && GameManager.Instance.IsGame() && _isMove == true)
            {
                //Debug.Log(name + targetObj.name);
                Vector3 targetDirection = targetObj.transform.position - transform.position + offsetPos;
                _rb.AddForce(targetDirection.normalized * _defaultAcceleration * _speed * accelerator, ForceMode.Acceleration);
                yield return new WaitForFixedUpdate();
            }
        }

        /// <summary>
        /// 停止&爆発&非表示
        /// </summary>
        /// <param name="canAddDamage">ダメージを与えるか</param>
        public IEnumerator ExplosionItemCol(bool canAddDamage)
        {
            _isExploded = true;
            //停止
            StopMove();

            if (canAddDamage)
            {
                //爆発半径内の惑星にダメージを与える
                AttackNearPlanet();
            }

            //縮小
            transform.DOScale(Vector3.zero, _scallingTime);

            //爆発パーティクル生成
            _explosionParticle.Emit(_explosionParticleEmitCount);
            //パーティクル生成中は処理停止
            yield return new WaitWhile(_explosionParticle.IsAlive);
            IsActive = false;
            gameObject.SetActive(false);
            //爆発通知
            ItemManager.Instance.ExplodeEvent();
            yield return null;
        }

        /// <summary>
        /// 爆発半径内の惑星にダメージを与える
        /// </summary>
        private void AttackNearPlanet()
        {
            Ray ray = new Ray(transform.position, transform.forward);
            foreach (RaycastHit hit in Physics.SphereCastAll(ray, _explosionRadius))
            {
                //惑星のタグなら
                if (hit.collider.tag == TagContainer.PLANET_TAG)
                {
                    //ダメージを与える
                    PlanetManager planet = hit.collider.GetComponent<PlanetManager>();
                    planet.TakeDamage(_power);
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            //爆発済ならリターン
            if (_isExploded == true) return;

            if (collision.gameObject.tag == TagContainer.PLANET_TAG)
            {
                //カウントダウン再開
                IsCountdown = true;
                //停止
                StopMove();
            }
        }
    }
}