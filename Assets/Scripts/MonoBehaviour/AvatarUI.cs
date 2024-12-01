using System.Collections;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class AvatarUI : MonoBehaviour
{
    [SerializeField] private TMP_Text devotionText;
    
    [Header("Nourishment Values")]
    [SerializeField] private Button nourishmentButton;
    [SerializeField] private int nourishmentCost;
    [SerializeField] private float nourishmentTime;
    
    private EntityManager _entityManager;
    private bool godlyNourishment;
    
    private void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    private void Update()
    {
        EntityQuery entityQuery = _entityManager.CreateEntityQuery(typeof(Avatar));
        Avatar avatar = entityQuery.GetSingleton<Avatar>();

        devotionText.text = avatar.Devotion.ToString();

        nourishmentButton.interactable = avatar.Devotion >= nourishmentCost && !godlyNourishment;
    }

    public void GodlyNourishment()
    {
        StartCoroutine(NourishFollowers());
    }

    private IEnumerator NourishFollowers()
    {
        float timer = 0;
       
        EnableNourishment();
        
        while (timer < nourishmentTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        
        DisableNourishment();
    }

    private void EnableNourishment()
    {
        godlyNourishment = true;
        RefRW<Avatar> avatar = _entityManager.CreateEntityQuery(typeof(Avatar)).GetSingletonRW<Avatar>();
        RefRW<Kitchen> kitchen = _entityManager.CreateEntityQuery(typeof(Kitchen)).GetSingletonRW<Kitchen>();

        kitchen.ValueRW.GodlyNourishment = true;
        avatar.ValueRW.Devotion -= nourishmentCost;
    }

    private void DisableNourishment()
    {
        RefRW<Kitchen> kitchen = _entityManager.CreateEntityQuery(typeof(Kitchen)).GetSingletonRW<Kitchen>();

        kitchen.ValueRW.GodlyNourishment = false;
        godlyNourishment = false;
    }
}
