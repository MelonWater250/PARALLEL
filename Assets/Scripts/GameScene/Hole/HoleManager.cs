using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Item;

namespace Hole
{
    //ブラックホールとホワイトホール間のアイテムの受け渡し
    public class HoleManager : MonoBehaviour
    {
        private List<ItemScript> _itemList = new List<ItemScript>();

        [SerializeField, Tooltip("発射先(相手の惑星)")]
        private GameObject _targetObj = null;

        [SerializeField]
        private WhiteHoleManager[] _whiteHoles = null;

        private void Update()
        {
            if( _itemList.Count > 0)
            {
                ////発射候補のアイテムリストにコピー
                //List<BombScript> fireItemList = new List<BombScript>(_itemList);
                //_itemList.Clear();

                //発射中でないWhiteHoleManagerを探す
                List<WhiteHoleManager> targetWhiteHoleList = new List<WhiteHoleManager>();
                foreach (WhiteHoleManager target in _whiteHoles)
                {
                    if (target.IsFire() == false)
                    {
                        targetWhiteHoleList.Add(target);
                    }
                }

                //見つかったら発射(候補の中からランダムで選出)
                //発射するアイテム以外は配列に戻す
                if (targetWhiteHoleList.Count > 0)
                {
                    int index = Random.Range(0, targetWhiteHoleList.Count);
                    WhiteHoleManager targetWhiteHole = _whiteHoles[index];
                    //targetWhiteHole.Fire(fireItemList.ToArray(), _targetObj);
                    targetWhiteHole.Fire(_itemList[0], _targetObj);
                    _itemList.RemoveAt(0);
                }
            }
        }

        //アイテムをリストに追加(ブラックホールから)
        public void AddItem(ItemScript item)
        {
            item.gameObject.SetActive(false);
            _itemList.Add(item);
        }
    }
}
