using System;
using TMPro;
using Unity.Entities;
using UnityEngine;

public class AvatarUI : MonoBehaviour
{
    [SerializeField] private TMP_Text devotionText;
    
    private EntityManager _entityManager;

    private void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy) return;

        EntityQuery entityQuery = _entityManager.CreateEntityQuery(typeof(Avatar));
        Avatar avatar = entityQuery.GetSingleton<Avatar>();

        devotionText.text = avatar.Devotion.ToString();
    }
}
