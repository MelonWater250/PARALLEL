using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Item;

namespace Hole
{
    public class WarpManager : MonoBehaviour
    {
        private List<ItemManager> _itemList = new List<ItemManager>();

        [SerializeField, Tooltip("発射先(相手の惑星)")]
        private GameObject _targetObj = null;

        [SerializeField]
        private WhiteHoleManager[] _whiteHoles = null;

        private void Update()
        {
            if( _itemList.Count > 0)
            {
                //発射候補のアイテムリストにコピー
                List<ItemManager> fireItemList = new List<ItemManager>(_itemList);
                _itemList.Clear();

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
                if (targetWhiteHoleList.Count > 0)
                {
                    int index = Random.Range(0, targetWhiteHoleList.Count);
                    WhiteHoleManager targetWhiteHole = _whiteHoles[index];
                    targetWhiteHole.Fire(fireItemList.ToArray(), _targetObj);
                }
                //見つからなかったら発射候補に戻す
                else
                {
                    foreach (ItemManager item in fireItemList)
                    {
                        _itemList.Add(item);
                    }
                }
                fireItemList.Clear();
            }
        }

        public void AddItem(ItemManager item)
        {
            item.gameObject.SetActive(false);
            _itemList.Add(item);
        }
    }
}
