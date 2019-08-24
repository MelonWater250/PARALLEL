using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Item;

namespace Cannon
{
    public class CannonManager : MonoBehaviour
    {
        const string TRIGGER_FIRE = "Fire";

        private int _attackPlayerNum= 1;
        
        //発射候補アイテムのリスト
        private List<ItemManager> _fireItemList = new List<ItemManager>();

        [SerializeField, Tooltip("アイテム再表示位置(発射位置)")]
        private Transform _firePos = null;

        //発射中か
        private bool _isFire = false;
        public bool IsFire() { return _isFire; }

        [SerializeField,Tooltip("発射先")]
        private GameObject _targetObj = null;

        [SerializeField, Tooltip("発射の時間間隔")]
        private float _fireIntervalTime = 0.1f;

        Animator _animator;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _attackPlayerNum = 1;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag(TagContainer.ITEM_TAG))
            {
                ItemManager item = other.GetComponent<ItemManager>();
                //相手の攻撃か動いてないなら無視
                if (item.IsMine(_attackPlayerNum) == false) return;
                if (item.IsMove() == false) return;

                //停止、非表示、発射候補リストに追加、アニメーション開始
                item.StopMove();
                item.gameObject.SetActive(false);
                _fireItemList.Add(item);
                _animator.SetTrigger(TRIGGER_FIRE);
            }
        }

        //アイテム発射
        public IEnumerator FireCol()
        {
            _isFire = true;
            List<ItemManager> fireItemList = new List<ItemManager>(_fireItemList);
            _fireItemList.Clear();
            foreach (ItemManager item in fireItemList)
            {
                item.gameObject.transform.position = _firePos.position;
                item.gameObject.SetActive(true);
                item.MoveItem(_targetObj);
                yield return new WaitForSeconds(_fireIntervalTime);
            }
            _isFire = false;
            fireItemList.Clear();
        }
    }
}