using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Planet;
using DG.Tweening;
using Item;

namespace Player
{
    /// <summary>
    /// 移動・回転、アイテムとの接触処理
    /// </summary>
    public abstract class PlayerBase : MonoBehaviour
    {
        //プレイヤーの番号
        [HideInInspector]
        protected int _playerNum = 1;

        [SerializeField, Tooltip("プレイヤーのオブジェクト")]
        protected GameObject _playerObj = null;

        [Header("パラメーター")]
        [SerializeField, Tooltip("移動速度")]
        protected float _moveSpeed = 60.0f;

        [SerializeField, Tooltip("回転速度")]
        protected float _rotateSpeed = 180.0f;

        [SerializeField, Tooltip("傾ける最大角度")]
        protected float _maxTiltAngle = 10.0f;

        [SerializeField, Tooltip("傾ける最大角度までかかる時間")]
        protected float _tiltTime = 1.0f;

        //操作できるか
        public bool CanMove { get; set; } = false;

        //移動回転方向
        protected Vector2 moveAxis;
        protected float rotateAxis;

        //球面移動の中心点
        protected PlanetManager _planetManager;

        [SerializeField, Tooltip("ブラックホールの位置")]
        protected GameObject[]  _blackHoles;

        protected virtual void Start()
        {
            //親からPlanetManagerを探す
            _planetManager = GetComponentInParent<PlanetManager>();
        }

        /// <summary>
        /// 移動
        /// </summary>
        /// <param name="moveAxis">移動方向</param>
        /// <param name="rotateAxis">回転方向(右回転なら+)</param>
        protected virtual void MovePlayer(Vector2 moveAxis, float rotateAxis)
        {
            if (CanMove == false) return;

            moveAxis.Normalize();

            //前後移動(X軸回転)
            transform.RotateAround(_planetManager.transform.position,                     //回転軸(惑星)
                _planetManager.transform.rotation * transform.localRotation * new Vector3(moveAxis.y, 0, 0),//回転方向(プレイヤーのローカル回転×入力方向)
                _moveSpeed * Time.deltaTime);   //回転量(入力量×スピード)


            //左右移動(Z軸回転)
            transform.RotateAround(_planetManager.transform.position,                     //回転軸(惑星)
                _planetManager.transform.rotation * transform.localRotation * new Vector3(0, 0, moveAxis.x), //回転方向(プレイヤーのローカル回転×入力方向)
                _moveSpeed * Time.deltaTime);  //回転量(入力量×スピード)

            //移動方向に傾ける
            _playerObj.transform.DOLocalRotate(new Vector3(moveAxis.y * _maxTiltAngle, 0, moveAxis.x * _maxTiltAngle), _tiltTime);


            //左右回転(Y軸)
            transform.Rotate(new Vector3(0, rotateAxis * _rotateSpeed * Time.deltaTime, 0));
        }

        //ライトの当たり判定
        protected virtual void OnTriggerEnter(Collider other)
        {
            //引き寄せ可能なアイテムなら近いほうのブラックホールまで飛ばす
            if (other.tag == TagContainer.ITEM_TAG)
            {
                ItemScript item = other.GetComponent<ItemScript>();
                bool canPull = (item.CanMove() && item.IsMine(_playerNum));
                if (canPull)
                {
                    //近いほうのブラックホールを算出
                    float bh1_distance = Vector3.Magnitude(_blackHoles[0].transform.position - transform.position);
                    float bh2_distance = Vector3.Magnitude(_blackHoles[1].transform.position - transform.position);

                    int nearBlackHoleIndex = bh1_distance <= bh2_distance ? 0 : 1;
                    //アイテムを飛ばす
                    ThrowItem(item, _blackHoles[nearBlackHoleIndex]);
                }
            }
        }

        /// <summary>
        /// アイテム受け渡し
        /// </summary>
        /// <param name="item">アイテム</param>
        /// <param name="target">発射先</param>
        protected virtual void ThrowItem(ItemScript item,GameObject target)
        {
            item.MoveItem(target);
        }
    }
}