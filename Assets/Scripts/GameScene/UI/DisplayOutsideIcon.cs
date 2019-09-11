using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Item;

/// <summary>
/// アイテムの画面外判定
/// 画面外のアイテムをアイコン表示
/// </summary>
public class DisplayOutsideIcon : MonoBehaviour
{
    [SerializeField,Tooltip("画面外に行くとアイコン表示するゲームオブジェクト")]
    private GameObject _iconObj= null;

    //アイコン制御用リスト
    private List<ItemIconScript> _icons = new List<ItemIconScript>();

    [SerializeField, Tooltip("表示する最大数")]
    private int _maxDisplayNum = 5;

    //アイコンを表示するか
    private bool _isDisplayIcon = true;

    [SerializeField, Tooltip("プレイヤーのカメラ")]
    private Camera _playerCamera = null;

    //惑星上のアイテム検索用
    [SerializeField]
    private List<ItemScript> items = new List<ItemScript>();
    public void AddItems(ItemScript item)
    {
        items.Add(item);
    }

    private void Start()
    {
        //アイコン生成(プーリング)
        for (int i = 0;i< _maxDisplayNum;i++)
        {
            //親は自分
            ItemIconScript icon = Instantiate(_iconObj,transform).GetComponent<ItemIconScript>();
            icon.Init();
            _icons.Add(icon);
        }
        _isDisplayIcon = true;
    }

    private void Update()
    {
        if (_isDisplayIcon == false) return;

        //アイコン表示するアイテム(画面外で近い順の_maxDisplayNumまで)
        //(値渡し)
        List<ItemScript> displayIconItems = new List<ItemScript>(OutSideItems(items));

        //近い順にソート(参照渡し)
        SortNearOrder(displayIconItems);

        for (int i = 0; i < displayIconItems.Count; i++)
        {
            Debug.Log("近い順 : " + i + " : " + displayIconItems[i].name);
        }

        //アイコン画像分ループして位置更新
        for (int i = 0; i < _icons.Count; i++) 
        {
            //対象がなかったらそれ以上のアイコンを非表示にしてbreak
            if (displayIconItems.Count <= i)
            {
                for(int j = i;j <_icons.Count;j++)
                {
                    //初期化&非表示
                    _icons[j].Init();
                }
                break;
            }
            // 画面内で対象を追跡
            Vector3 viewportPos = _playerCamera.WorldToViewportPoint(displayIconItems[i].transform.position);
            //画面内にクランプする
            viewportPos.x = Mathf.Clamp01(viewportPos.x);
            viewportPos.y = Mathf.Clamp01(viewportPos.y);
            //近い順のアイコンの数だけそのアイコンの位置を変更する
            _icons[i].UpdateUIPos(_playerCamera.rect,_playerCamera.transform.position, viewportPos, displayIconItems[i]);
        }
    }

    /// <summary>
    /// 画面外のアクティブのアイテムのみ返す
    /// </summary>
    /// <param name="maxindex">返す最大数</param>
    /// <returns>画面外のアイテム</returns>
    private List<ItemScript> OutSideItems(List<ItemScript> items)
    {
        List<ItemScript> outSideItems = new List<ItemScript>();
        for (int i =0; i<items.Count;i++)
        {
            //アイテムのビューポート上の位置
            Vector3 viewportPoint = _playerCamera.WorldToViewportPoint(items[i].transform.position);
            Rect rect = _playerCamera.rect;

            //画面外か
            bool isOutSide = rect.Contains(viewportPoint) == false;
            //アクティブか
            bool isActive = items[i].IsActive;
            if (isOutSide && isActive)
            {
                outSideItems.Add(items[i]);
            }
        }
        return outSideItems;
    }

    /// <summary>
    /// 近い順にソート
    /// </summary>
    /// <returns></returns>
    private void SortNearOrder(List<ItemScript> items)
    {
        //近い順(昇順)にソート
        items.Sort((a, b) => 
        {
            float DistanceA = (a.gameObject.transform.position - _playerCamera.transform.position).magnitude;
            float DistanceB = (b.gameObject.transform.position - _playerCamera.transform.position).magnitude;
            return (int)(DistanceA - DistanceB);
        });
    }
    
}
