using UnityEngine;

[RequireComponent(typeof(IngredientInteractable))]
public class IngredientIcon : MonoBehaviour
{
    public Vector3 offset = new Vector3(0f, 1.2f, 0f);
    public float iconScale = 0.2f;
    public string sortingLayerName = "UI";
    public int sortingOrder = 10;
    public Sprite bubbleIcon;
    public Vector3 ingredientOffset = Vector3.zero;

    public Sprite meatIcon;
    public Sprite fishIcon;
    public Sprite potatoIcon;
    public Sprite onionIcon;
    public Sprite tomatoIcon;
    public Sprite herbIcon;

    private IngredientInteractable ingredient;
    private Transform iconRoot;
    private SpriteRenderer bubbleRenderer;
    private SpriteRenderer ingredientRenderer;

    private void Awake()
    {
        ingredient = GetComponent<IngredientInteractable>();
        CreateOrFindIcon();
        UpdateIconSprite();
    }

    private void LateUpdate()
    {
        if (iconRoot == null)
        {
            return;
        }

        Camera cam = Camera.main;
        if (cam == null)
        {
            return;
        }

        // 항상 카메라를 바라보도록 회전
        iconRoot.rotation = cam.transform.rotation;
    }

    public void UpdateIconSprite()
    {
        if (ingredientRenderer == null)
        {
            return;
        }

        ingredientRenderer.sprite = GetIconForType(ingredient != null ? ingredient.ingredientType : IngredientType.Meat);
        ingredientRenderer.enabled = ingredientRenderer.sprite != null;
    }

    private void CreateOrFindIcon()
    {
        Transform existing = transform.Find("IngredientIconRoot");
        if (existing != null)
        {
            iconRoot = existing;
            bubbleRenderer = iconRoot.Find("BubbleIcon")?.GetComponent<SpriteRenderer>();
            ingredientRenderer = iconRoot.Find("IngredientIcon")?.GetComponent<SpriteRenderer>();
            EnsureRenderers();
            return;
        }

        GameObject rootObj = new GameObject("IngredientIconRoot");
        rootObj.transform.SetParent(transform);
        rootObj.transform.localPosition = offset;
        rootObj.transform.localRotation = Quaternion.identity;
        rootObj.transform.localScale = Vector3.one * iconScale;

        iconRoot = rootObj.transform;

        GameObject bubbleObj = new GameObject("BubbleIcon");
        bubbleObj.transform.SetParent(iconRoot);
        bubbleObj.transform.localPosition = Vector3.zero;
        bubbleObj.transform.localRotation = Quaternion.identity;
        bubbleObj.transform.localScale = Vector3.one;
        bubbleRenderer = bubbleObj.AddComponent<SpriteRenderer>();
        SetupRenderer(bubbleRenderer, sortingOrder);
        bubbleRenderer.sprite = bubbleIcon;

        GameObject ingredientObj = new GameObject("IngredientIcon");
        ingredientObj.transform.SetParent(iconRoot);
        ingredientObj.transform.localPosition = ingredientOffset;
        ingredientObj.transform.localRotation = Quaternion.identity;
        ingredientObj.transform.localScale = Vector3.one;
        ingredientRenderer = ingredientObj.AddComponent<SpriteRenderer>();
        SetupRenderer(ingredientRenderer, sortingOrder + 1);
    }

    private void EnsureRenderers()
    {
        if (bubbleRenderer == null)
        {
            Transform bubble = iconRoot.Find("BubbleIcon");
            if (bubble == null)
            {
                bubble = new GameObject("BubbleIcon").transform;
                bubble.SetParent(iconRoot);
                bubble.localPosition = Vector3.zero;
                bubble.localRotation = Quaternion.identity;
                bubble.localScale = Vector3.one;
            }
            bubbleRenderer = bubble.GetComponent<SpriteRenderer>();
            if (bubbleRenderer == null)
            {
                bubbleRenderer = bubble.gameObject.AddComponent<SpriteRenderer>();
            }
        }

        if (ingredientRenderer == null)
        {
            Transform icon = iconRoot.Find("IngredientIcon");
            if (icon == null)
            {
                icon = new GameObject("IngredientIcon").transform;
                icon.SetParent(iconRoot);
                icon.localPosition = ingredientOffset;
                icon.localRotation = Quaternion.identity;
                icon.localScale = Vector3.one;
            }
            ingredientRenderer = icon.GetComponent<SpriteRenderer>();
            if (ingredientRenderer == null)
            {
                ingredientRenderer = icon.gameObject.AddComponent<SpriteRenderer>();
            }
        }

        SetupRenderer(bubbleRenderer, sortingOrder);
        SetupRenderer(ingredientRenderer, sortingOrder + 1);

        bubbleRenderer.sprite = bubbleIcon;
    }

    private void SetupRenderer(SpriteRenderer renderer, int order)
    {
        if (renderer == null)
        {
            return;
        }

        renderer.sortingLayerName = sortingLayerName;
        renderer.sortingOrder = order;
    }

    private void OnValidate()
    {
        if (iconRoot != null)
        {
            EnsureRenderers();
            UpdateIconSprite();
        }
    }

    private Sprite GetIconForType(IngredientType type)
    {
        switch (type)
        {
            case IngredientType.Meat:
                return meatIcon;
            case IngredientType.Fish:
                return fishIcon;
            case IngredientType.Potato:
                return potatoIcon;
            case IngredientType.Onion:
                return onionIcon;
            case IngredientType.Tomato:
                return tomatoIcon;
            case IngredientType.Herb:
                return herbIcon;
            default:
                return meatIcon;
        }
    }
}

