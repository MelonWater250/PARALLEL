using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hole;

namespace Item
{
    //ゲームシーン上のアイテム管理
    public class ItemManager : SingletonMonoBehaviour<ItemManager>
    {
        [Header("Item_item")]
        [SerializeField, Tooltip("アイテム(爆弾)")]
        private GameObject _item = null;

        [SerializeField, Tooltip("最初に生成する数(プーリング数)")]
        private int _firstInstantiateItemAmount = 20;

        //生成した数保管用
        private int _instantiateCount = 0;

        [SerializeField, Tooltip("最初にホワイトホールから出現させる数(1惑星分)")]
        private int _firstUseItemAmount = 5;

        [SerializeField, Tooltip("爆弾を増やすのに必要な爆発数")]
        private int _increaseExplodeItemAmount = 2;

        [SerializeField, Tooltip("爆弾を増やす数")]
        private int _increaseItemAmount = 2;

        //爆発した数
        private int _explodeItemCount = 0;

        [SerializeField, Tooltip("ホールマネージャー(初回アイテム生成用)")]
        private HoleManager _holeManager_1 = null;
        [SerializeField, Tooltip("ワープマネージャー(初回アイテム生成用)")]
        private HoleManager _holeManager_2 = null;

        //管理用リスト
        private List<ItemScript> _itemList = new List<ItemScript>();

        //初回生成＆プーリング&初回出現
        public void InitItem()
        {
            InstantiateItem(_firstInstantiateItemAmount);

            //両方のプレイヤーのホワイトホールにアイテムを渡す
            AddToWhiteHole(_firstUseItemAmount, 1);
            AddToWhiteHole(_firstUseItemAmount, 2);
        }

        /// <summary>
        /// 爆弾生成＆プーリング
        /// </summary>
        /// <param name="spawnNum">生成数</param>
        /// <returns>生成したアイテムのID</returns>
        private int[] InstantiateItem(int spawnNum = 1)
        {
            int[] spawnItem = new int[spawnNum];
            //生成
            for (int i = 0; i < spawnNum; i++)
            {
                ItemScript item = Instantiate(_item).GetComponent<ItemScript>();
                //ID登録&初期化
                item.gameObject.SetActive(false);
                item.Init(_instantiateCount);
                _instantiateCount++;
                item.name += ":" + _instantiateCount;
                _itemList.Add(item);

                spawnItem[i] = item.ItemID; 
            }
            return spawnItem;
        }

        /// <summary>
        /// ワープマネージャーに受け渡し(無ければ生成)
        /// </summary>
        /// <param name="addAmount">渡す数</param>
        /// <param name="whiteHoleNum">渡すホワイトホール(1か2)</param>
        private void AddToWhiteHole(int addAmount,int whiteHoleNum)
        {
            for (int i = 0; i < addAmount; i++)
            {
                int itemId = IdleItemID();

                //初期化
                _itemList[itemId].Init();
                _itemList[itemId].IsActive = true;

                if (whiteHoleNum == 1)
                {
                    _holeManager_1.AddItem(_itemList[itemId]);
                }
                else if (whiteHoleNum == 2)
                {
                    _holeManager_2.AddItem(_itemList[itemId]);
                }
            }
        }

        /// <summary>
        /// 非表示(待機中)のアイテムがあればIDを返す無ければ生成する
        /// </summary>
        private int IdleItemID()
        { 
            //非表示のものを探す
            foreach (ItemScript item in _itemList)
            {
                if (item.IsActive == false)
                {
                    return item.ItemID;
                }
            }

            //なければ1つ生成して番号を返す
            return InstantiateItem()[0];
        }

        /// <summary>
        /// 爆発処理
        /// (爆発した時にアイテムから呼ばれる)
        /// </summary>
        public void ExplodeEvent()
        {
            if (GameManager.Instance.IsGame() == false) return;

            //爆発した数を足す
            _explodeItemCount++;

            //ホワイトホールに渡す数
            int spawnNum = 1;

            //必要爆発数で割り切れる＝必要爆発数毎に爆弾追加
            if (_explodeItemCount % _increaseExplodeItemAmount == 0)
            {
                spawnNum += _increaseItemAmount;
            }
            
            for (int i = 0; i < spawnNum; i++)
            {
                //渡すホワイトホール(ランダム)
                //Random.Range(1, 3)で1か2のどちらか
                int whiteHoleNum = Random.Range(1, 3);
                AddToWhiteHole(1,whiteHoleNum);
            }
        }

        //全てのアイテムを爆発させて止める
        public void ExplosionAllItem()
        {
            foreach (ItemScript item in _itemList)
            {
                StartCoroutine(item.ExplosionItemCol(false));
            }
        }
    }
}