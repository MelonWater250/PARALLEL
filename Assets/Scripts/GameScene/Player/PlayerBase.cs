using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Planet;
using DG.Tweening;
using Item;

namespace Player
{
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

        protected PlanetManager _planetManager;

        //所持中のアイテム
        protected List<BombScript> _itemList = new List<BombScript>();

        [SerializeField, Tooltip("所持できるアイテムの最大数")]
        protected int _maxItemNum = 5;
        protected virtual bool CanCarry() { return _maxItemNum >= _itemList.Count + 1; }

        [SerializeField, Tooltip("アイテムを引き寄せるピボットの相対位置")]
        protected Vector3 _pivotOffset = new Vector3(0, -0.1f, 0);

        [SerializeField, Tooltip("アイテムを引き寄せるスピード(倍)")]
        protected float _accelerator = 3.0f;

            [SerializeField, Tooltip("アイテムを引き渡す時間間隔")]
            protected float _throwIntervalTime = 0.1f;

        [SerializeField, Tooltip("ブラックホールの位置")]
        protected GameObject[]  _blackHoles;

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


        protected virtual void OnTriggerEnter(Collider other)
        {
            switch (other.tag)
            {
                //アイテムなら引き寄せる
                case TagContainer.ITEM_TAG:
                    BombScript item = other.GetComponent<BombScript>();
                    bool canPull = (item.CanMove() && item.IsMine(_playerNum));
                    if (canPull && CanCarry())
                    {
                        PullItem(item);
                    }
                    break;

                //大砲なら受け渡す
                case TagContainer.CANNON_TAG:
                    ThrowItem(other.gameObject);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// アイテム引き寄せ
        /// </summary>
        /// <param name="item">アイテム</param>
        protected virtual void PullItem(BombScript item)
        {
            //リストに追加と移動
            item.MoveItem(gameObject, _pivotOffset, _accelerator);
            _itemList.Add(item);
            //縮小
            item.ShrinkItem(time: null, OnCompleteAction:
                () =>
                {
                    //一番近いブラックホールを探す
                    GameObject nearBlackHole = _blackHoles[0];
                    for(int i=1;i<_blackHoles.Length;i++)
                    {
                        if(nearBlackHole.transform.position.magnitude > _blackHoles[i].transform.position.magnitude)
                        {
                            nearBlackHole = _blackHoles[i];
                        }
                    }
                    ThrowItem(nearBlackHole);
                });

        }

        /// <summary>
        /// アイテム受け渡し
        /// </summary>
        /// <param name="target">大砲</param>
        protected virtual void ThrowItem(GameObject target)
        {
            if (_itemList.Count > 0)
            {
                List<BombScript> itemList = new List<BombScript>(_itemList);
                _itemList.Clear();
                StartCoroutine(ThrowItemCol(new List<BombScript>(itemList), target));
            }
        }

        //アイテム発射
        protected virtual IEnumerator ThrowItemCol(List<BombScript> itemList, GameObject target)
        {
            foreach (BombScript item in itemList)
            {
                item.transform.position = transform.position;
                item.gameObject.SetActive(true);
                item.MoveItem(target);
                //拡大
                item.ExpandingItem();
                yield return new WaitForSeconds(_throwIntervalTime);
            }
        }
    }
}