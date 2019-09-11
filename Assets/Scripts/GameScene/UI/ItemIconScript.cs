using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Item;

/// <summary>
/// アイテムのアイコン処理
/// </summary>
public class ItemIconScript : MonoBehaviour
{
    //使っているか
    private bool _isActive = false;
    public bool IsActive() { return _isActive; }

    [Header("IconImage")]
    [SerializeField, Tooltip("アイテムアイコン(初期)")]
    private Image _normalImage = null;

    [SerializeField, Tooltip("アイテムアイコン(爆発)")]
    private Image _explodeImage = null;

    [Header("IconColor")]
    [SerializeField, Tooltip("アイコンの初期色")]
    private Color _normalColor = Color.white;

    [SerializeField, Tooltip("アイコンの爆発色")]
    private Color _explodeColor = Color.red;

    [Header("IconSize")]
    [SerializeField, Tooltip("アイコンの最小サイズ")]
    private Vector2 _minSize = new Vector2(20, 20);

    [SerializeField, Tooltip("アイコンの最大サイズ")]
    private Vector2 _maxSize = new Vector2(50, 50);

    [Header("IconSizeByDistance")]
    [SerializeField, Tooltip("アイコンの最小サイズ時の距離")]
    private float _maxDistance = 10f;

    private void Start()
    {
        _normalImage.color = _normalColor;
        _normalImage.rectTransform.localScale = _minSize;
    }

    //初期化
    public void Init()
    {
        gameObject.SetActive(false);
        _isActive = false;
    }

    /// <summary>
    /// アイテムの情報を表示
    /// </summary>
    /// <param name="viewportPos">画面内の位置</param>
    public void UpdateUIPos(Rect rect,Vector3 cameraPos,Vector3 viewportPos,ItemScript item)
    {
        _isActive = true;
        gameObject.SetActive(true);
        _normalImage.rectTransform.anchoredPosition = Rect.NormalizedToPoint(rect, viewportPos);
        //距離が近いほど大きくする
        float distance = (item.transform.position - cameraPos).magnitude;
        Debug.Log(distance);
        Debug.Log(Mathf.Clamp01(distance / _maxDistance));
        _normalImage.rectTransform.localScale = Vector3.Lerp(_maxSize, _minSize, Mathf.Clamp01(distance / _maxDistance));
        Debug.Log(_normalImage.rectTransform.localScale);
    }
}
