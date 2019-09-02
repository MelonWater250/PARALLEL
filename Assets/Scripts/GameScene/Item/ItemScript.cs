//using Planet;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using DG.Tweening;
//using System;

//namespace Item
//{
//    public class BombScript : MonoBehaviour
//    {
//        //動作中か
//        [HideInInspector]
//        public bool IsActive = false;

//        //攻撃者のプレイヤー番号(スポーン時に書き換え)
//        private int _attackPlayerNum = 1;
//        public void SetPlayerNum(int playerNum) { _attackPlayerNum = playerNum; }

//        public bool IsMine(int playerNum) { return playerNum == _attackPlayerNum ? true : false; }

//        //アイテムの固有ID
//        public int ItemID;

//        //初期スケール値
//        private Vector3 _defaultScale = new Vector3(1, 1, 1);

//        [SerializeField, Tooltip("アイテム拡縮にかかる時間")]
//        private float _scallingTime = 0.5f;

//        public enum ItemTypeEnum
//        {
//            Stone = 0,
//            Bomb
//        }
//        public ItemTypeEnum ItemType = ItemTypeEnum.Stone;

//        [SerializeField, Tooltip("攻撃力")]
//        private float _power = 10.0f;

//        [SerializeField, Tooltip("移動速度")]
//        private float _speed = 5.0f;

//        private bool _isMove = false;
//        public bool IsMove() { return _isMove; }

//        public void StopMove() { _isMove = false; }

//        [SerializeField, Tooltip("爆発パーティクル")]
//        private ParticleSystem _explosionParticle = null;

//        [SerializeField, Tooltip("爆発パーティクルの生成数")]
//        private int _explosionParticleEmitCount = 30;

//        [SerializeField, Tooltip("爆発時間")]
//        private float _explosionTime = 3.0f;

//        private void Start()
//        {
//            _defaultScale = transform.localScale;
//            IsActive = true;
//        }

//        /// <summary>
//        /// 移動
//        /// </summary>
//        /// <param name="targetObj">移動先</param>
//        /// <param name="offsetPos">移動先のピボットの相対位置</param>
//        /// <param name="accelerator">速度の倍率</param>
//        public void MoveItem(GameObject targetObj, Vector3 ? offset = null, float accelerator = 1.0f)
//        {
//            if (IsActive == false) return;
//            if (targetObj == null) return;
//            if (offset == null) offset = Vector3.zero;

//            StopMove();
//            transform.parent = null;
//            StartCoroutine(MoveItemCol(targetObj,(Vector3)offset, accelerator));
//        }
        
//        private IEnumerator MoveItemCol(GameObject targetObj, Vector3 offsetPos, float accelerator = 1.0f)
//        {
//            if (GameManager.Instance.IsGame() == false) yield break;

//            float newSpeed = _speed * accelerator;
//                _isMove = true;
//            while (IsMove() && GameManager.Instance.IsGame())
//            {
//                transform.position += ((targetObj.transform.position + targetObj.transform.TransformVector(offsetPos)) - transform.position).normalized * newSpeed * Time.deltaTime;
//                yield return null;
//            }
//        }

//        /// <summary>
//        /// 拡大
//        /// </summary>
//        public void ExpandingItem(float? time = null, Action OnCompleteAction = null)
//        {
//            if (time == null) time = _scallingTime;
//            transform.DOScale(_defaultScale, (float)time).OnComplete(() => OnCompleteAction());
//        }

//        /// <summary>
//        /// 縮小
//        /// </summary>
//        public void ShrinkItem(float? time = null, Action OnCompleteAction = null)
//        {
//            if (time == null) time = _scallingTime;
//            transform.DOScale(Vector3.zero, (float)time).OnComplete(() => OnCompleteAction());
//        }

//        /// <summary>
//        /// 停止&爆発&非表示
//        /// </summary>
//        public IEnumerator ExplosionItemCol()
//        {
//            StopMove();
//            //縮小
//            ShrinkItem(_explosionTime);
//            //爆発パーティクル生成
//            ParticleSystem particle = Instantiate(_explosionParticle,transform.position,Quaternion.identity);
//            particle.Emit(_explosionParticleEmitCount);
//            //パーティクル生成中は処理停止
//            yield return new WaitWhile(particle.IsAlive);
//            Destroy(particle.gameObject);
//            IsActive = false;
//            gameObject.SetActive(false);
//            yield return null;
//        }

//        private void OnTriggerEnter(Collider other)
//        {
//            switch (other.tag)
//            {
//                case TagContainer.PLANET_TAG:
//                    PlanetManager planet = other.GetComponent<PlanetManager>();

//                    //相手の惑星なら
//                    if (IsMine(planet.PlayerNum) == false)
//                    {
//                        planet.TakeDamage(_power);
//                        StartCoroutine(ExplosionItemCol());
//                    }
//                    break;
                    
//                default:
//                    break;
//            }
//        }
//    }
//}