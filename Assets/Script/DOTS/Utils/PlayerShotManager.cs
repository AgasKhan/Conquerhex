using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class PlayerShotManager : MonoBehaviour
{
    [SerializeField] private GameObject shootingPopupPrefab;
    private void Start()
    {
        World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PlayerShootingSystem>();

        PlayerShootingSystem.OnShoot += PlayerShootingSystem_OnShoot;
    }
    
    private void PlayerShootingSystem_OnShoot(object sender, System.EventArgs e)
    {
        Unity.Entities.Entity playerEntity = (Unity.Entities.Entity)sender;

        LocalTransform localTransform = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<LocalTransform>(playerEntity);

        Instantiate(shootingPopupPrefab, localTransform.Position, quaternion.identity);
    }
}
