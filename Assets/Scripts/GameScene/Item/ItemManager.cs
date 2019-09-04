using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hole;

namespace Item
{
    //ゲームシーン上のアイテム管理
    public class ItemManager : SingletonMonoBehaviour<ItemManager>
    {
        [Header("Item_Bomb")]
        [SerializeField, Tooltip("生成するアイテム(爆弾)の数")]
        private int _itemAmountBomb = 15;

        [SerializeField, Tooltip("アイテム(爆弾)")]
        private GameObject _itemBomb = null;

        [SerializeField, Tooltip("最初にホワイトホールから生成する数\n(_itemAmountBombの2分の1以下)")]
        private int _firstSpawnAmount = 5;

        [SerializeField,Tooltip("ワープマネージャー(初回アイテム生成用)")]
        private HoleManager _warpManager_1 = null;
        [SerializeField, Tooltip("ワープマネージャー(初回アイテム生成用)")]
        private HoleManager _warpManager_2 = null;

        //管理用リスト
        private List<ItemScript> _itemBombList = new List<ItemScript>();

        private void Start()
        {
            InstantiateItem();
        }

        //生成＆管理用リスト追加
        private void InstantiateItem()
        {
            //生成
            for (int i = 0; i < _itemAmountBomb; i++)
            {
                ItemScript item = Instantiate(_itemBomb).GetComponent<ItemScript>();
                item.Init(i);
                item.gameObject.SetActive(false);
                _itemBombList.Add(item);
            }

            //ワープマネージャーに受け渡し(初回生成)
            for (int i = 0; i < _firstSpawnAmount * 2; i++)
            {
                //偶数と奇数交互に代入
                if (i % 2 == 0)
                {
                    _warpManager_1.AddItem(_itemBombList[i]);
                }
                else
                {
                    _warpManager_2.AddItem(_itemBombList[i]);
                }

            }
        }

        /// <summary>
        /// 非表示(待機中)のアイテムがあれば返す
        /// </summary>
        public ItemScript IdleItem()
        {
            List<ItemScript> idleItemList = new List<ItemScript>();
            //非表示のものを探す
            foreach (ItemScript item in _itemBombList)
            {
                if (item.IsActive == false)
                {
                    idleItemList.Add(item);
                }
            }
            //無かったらnullリターン
            if (idleItemList.Count <= 0) return null;

            int index = Random.Range(0, idleItemList.Count);
            return idleItemList.ToArray()[index];
        }

        //全てのアイテムを爆発させて止める
        public void ExplosionAllItem()
        {
            foreach (ItemScript item in _itemBombList)
            {
                StartCoroutine(item.ExplosionItemCol());
            }
        }
    }
}