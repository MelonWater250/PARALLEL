using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Item;
using Planet;

namespace Hole
{
    //ブラックホールとホワイトホール間のアイテムの受け渡し
    public class HoleManager : MonoBehaviour
    {
        //発射予定のアイテムのリスト(ブラックホールから受け取り)
        private List<ItemScript> _itemList = new List<ItemScript>();

        [SerializeField, Tooltip("発射先(相手の惑星)")]
        private GameObject _targetObj = null;

        [SerializeField, Tooltip("ブラックホールとつながっているホワイトホール")]
        private WhiteHoleManager[] _whiteHoles = null;

        [SerializeField, Tooltip("ホワイトホールまでワープする時間")]
        private float _warpIntervalTime = 2.0f;

        //攻撃者のプレイヤー番号呼び出し用
        PlanetManager _planetManager = null;

        private void Start()
        {
            _planetManager = GetComponentInParent<PlanetManager>();
            //StartCoroutine(WarpItemUpdateCol());
        }

        //Itemをホワイトホールに受け渡す(毎フレーム実行)
        private void  Update/*IEnumerator WarpItemUpdateCol*/()
        {
            //ゲーム中ならループ
            if (GameManager.Instance.IsGame() == true)
            {
                //発射予定のアイテムが1つ以上あるなら
                if (_itemList.Count > 0)
                {
                    //発射準備のアイテム配列に代入してクリア
                    List<ItemScript> fireItems = new List<ItemScript>(_itemList);
                    _itemList.Clear();

                    //すべてのアイテムを発射準備
                    for (int i = 0; i < fireItems.Count; i++)
                    {
                        //発射中でないホワイトホールを探す
                        List<WhiteHoleManager> targetWhiteHoleList = new List<WhiteHoleManager>();
                        for (int j = 0; j < _whiteHoles.Length; j++)
                        {
                            //発射中じゃないなら
                            if (_whiteHoles[j].IsFire() == false)
                            {
                                //リストに追加
                                targetWhiteHoleList.Add(_whiteHoles[j]);
                            }
                        }

                        //発射中じゃないホワイトホールが見つかったら発射(候補の中からランダムで選出)
                        //発射するアイテム以外は配列に戻す
                        if (targetWhiteHoleList.Count > 0)
                        {
                            int index = Random.Range(0, targetWhiteHoleList.Count);
                            targetWhiteHoleList[index].Fire(fireItems[i], _targetObj, _planetManager.PlayerNum);
                        }
                        else
                        {
                            //発射中じゃないホワイトホールが見つからなかったら
                            //発射予定のアイテムのリストに返す
                            fireItems[i].gameObject.SetActive(false);
                            _itemList.Add(fireItems[i]);
                        }
                    }
                }
                //一定時間処理停止
                //yield return new WaitForSeconds(_warpIntervalTime);
                //yield return null;
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