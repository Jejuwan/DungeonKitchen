using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PlateInteractable : MonoBehaviour, IInteractable
{
    [Serializable]
    public class RecipeEntry
    {
        public string dishName;
        public IngredientType[] requiredIngredients;
        public GameObject resultPrefab;
    }

    [Header("Plate Slots")]
    public Transform ingredientRoot;
    public Transform resultRoot;

    [Header("Recipe")]
    public bool requireCookedIngredient = true;
    public RecipeEntry[] recipes;

    public Transform Root => transform;
    public bool CanPickUp => true;
    public bool CanPlace => false;

    private Rigidbody rb;
    private Collider col;
    private readonly List<IngredientType> placedIngredients = new List<IngredientType>();
    private bool hasDishResult;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        EnsureRoots();
    }

    public void PickUp(Transform holdPoint)
    {
        if (holdPoint == null)
        {
            return;
        }

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        col.enabled = false;

        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop(Vector3 dropPosition)
    {
        // 접시는 바닥 드롭을 허용하지 않음
    }

    public bool Place(IInteractable item)
    {
        return false;
    }

    public bool TryAddIngredient(IInteractable item)
    {
        if (hasDishResult)
        {
            return false;
        }

        IngredientInteractable ingredient = item as IngredientInteractable;
        if (ingredient == null)
        {
            return false;
        }

        if (requireCookedIngredient && (!ingredient.isCooked || ingredient.isBurnt))
        {
            return false;
        }

        ingredient.PickUp(ingredientRoot);
        ApplyIngredientLayout(ingredient.transform, placedIngredients.Count);
        placedIngredients.Add(ingredient.ingredientType);
        TryCreateDishResult();
        return true;
    }

    private void ApplyIngredientLayout(Transform ingredientTransform, int index)
    {
        if (ingredientTransform == null)
        {
            return;
        }

        float radius = 0.08f;
        float angle = index * 45f * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
        ingredientTransform.localPosition = offset + Vector3.up * (0.01f * index);
        ingredientTransform.localRotation = Quaternion.identity;
    }

    private void TryCreateDishResult()
    {
        if (recipes == null || recipes.Length == 0)
        {
            return;
        }

        for (int i = 0; i < recipes.Length; i++)
        {
            RecipeEntry recipe = recipes[i];
            if (!IsRecipeMatch(recipe))
            {
                continue;
            }

            BuildDish(recipe);
            return;
        }
    }

    private bool IsRecipeMatch(RecipeEntry recipe)
    {
        if (recipe == null || recipe.resultPrefab == null || recipe.requiredIngredients == null)
        {
            return false;
        }

        if (recipe.requiredIngredients.Length != placedIngredients.Count)
        {
            return false;
        }

        Dictionary<IngredientType, int> required = new Dictionary<IngredientType, int>();
        for (int i = 0; i < recipe.requiredIngredients.Length; i++)
        {
            IngredientType type = recipe.requiredIngredients[i];
            int count;
            required.TryGetValue(type, out count);
            required[type] = count + 1;
        }

        for (int i = 0; i < placedIngredients.Count; i++)
        {
            IngredientType type = placedIngredients[i];
            int count;
            if (!required.TryGetValue(type, out count))
            {
                return false;
            }

            count--;
            if (count == 0)
            {
                required.Remove(type);
            }
            else
            {
                required[type] = count;
            }
        }

        return required.Count == 0;
    }

    private void BuildDish(RecipeEntry recipe)
    {
        for (int i = ingredientRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(ingredientRoot.GetChild(i).gameObject);
        }

        GameObject dish = Instantiate(recipe.resultPrefab, resultRoot, false);
        dish.transform.localPosition = Vector3.zero;
        dish.transform.localRotation = Quaternion.identity;

        hasDishResult = true;
    }

    private void EnsureRoots()
    {
        if (ingredientRoot == null)
        {
            GameObject ingredientRootObject = new GameObject("IngredientRoot");
            ingredientRootObject.transform.SetParent(transform, false);
            ingredientRoot = ingredientRootObject.transform;
        }

        if (resultRoot == null)
        {
            GameObject resultRootObject = new GameObject("ResultRoot");
            resultRootObject.transform.SetParent(transform, false);
            resultRoot = resultRootObject.transform;
        }
    }
}
