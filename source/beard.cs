using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using System.Collections.Generic;
using ItemAPI;
using MultiplayerBasicExample;

namespace knifeJam
{

    class Beard : AdvancedGunBehaviour
    {
        public static void Add()
        {

            Gun gun = ETGMod.Databases.Items.NewGun("Cpt Boombeard's Cutlass", "brd");

            Game.Items.Rename("outdated_gun_mods:cpt_boombeard's_cutlass", "ski:cpt_boombeard's_cutlass");
            gun.gameObject.AddComponent<Beard>();
            gun.SetShortDescription("Now with Real Dynamite!");
            gun.SetLongDescription("Infamous Captain Boombeard was known for interlacing his beard with explosives to terrify his opponents. Unfortunatly for him this idea was terrible as he managed to burn off his beard." +
                "\n\nGive a hearty chuckle to alert your crew its time to have some fun! ");

            gun.SetupSprite(null, "brd_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(481) as Gun, true, false);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.gunHandedness = GunHandedness.OneHanded;
            gun.reloadTime = .1f;
            gun.DefaultModule.cooldownTime = 1f;
            gun.DefaultModule.numberOfShotsInClip = 20;
            gun.SetBaseMaxAmmo(400);

            gun.quality = PickupObject.ItemQuality.B;
            gun.encounterTrackable.EncounterGuid = "Summon the megaladon!";

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage *= 3f;
            projectile.baseData.range = 1;
            projectile.baseData.speed *= 1f;
            projectile.transform.parent = gun.barrelOffset;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = "black_revolver_big";
            ProjectileSlashingBehaviour slash = projectile.gameObject.AddComponent<ProjectileSlashingBehaviour>();
            slash.SlashDamage = 20;
            slash.SlashRange = 3;
            slash.SlashDimensions = 90f;
            slash.InteractMode = SlashDoer.ProjInteractMode.DESTROY;
            Beard.BuildPrefab();
            tk2dSpriteAnimationClip fireClip = gun.sprite.spriteAnimator.GetClipByName("brd_fire");
            float[] offsetsX = new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
            float[] offsetsY = new float[] { -0f, -1f, -1f, -1f, -1f };
            for (int i = 0; i < offsetsX.Length && i < offsetsY.Length && i < fireClip.frames.Length; i++)
            {
                int id = fireClip.frames[i].spriteId;
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position0.x += offsetsX[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position0.y += offsetsY[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position1.x += offsetsX[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position1.y += offsetsY[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position2.x += offsetsX[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position2.y += offsetsY[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position3.x += offsetsX[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position3.y += offsetsY[i];
            }
            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }
        public System.Random laugh = new System.Random();
        protected override void OnPickedUpByPlayer(PlayerController player)
        {
            this.gun.sprite.renderer.enabled = false;
            player.GunChanged += this.OnGunChanged;
            base.OnPickedUpByPlayer(player);
        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            ProjectileSlashingBehaviour slash = projectile.gameObject.AddComponent<ProjectileSlashingBehaviour>();
            slash.SlashDamage = 30;
            slash.SlashRange = 2.5f;
            slash.SlashDimensions = 30f;
            slash.InteractMode = SlashDoer.ProjInteractMode.DESTROY;
            base.PostProcessProjectile(projectile);
            
        }
        protected override void OnPostDrop(GameActor owner)
        {
            PlayerController player = (PlayerController)owner;
            try
            {
                player.GunChanged -= this.OnGunChanged;
                base.OnPostDrop(player);
            }
            catch (Exception e)
            {
                Tools.Print("beard drop", "FFFFFF", true);
                Tools.PrintException(e);
            }
        }
        private void OnGunChanged(Gun oldGun, Gun newGun, bool arg3)
        {
            try
            {
                if (newGun == this.gun)
                {
                    this.gun.sprite.renderer.enabled = false;
                }
            }
            catch (Exception e)
            {
                Tools.Print("Beard change", "FFFFFF", true);
                Tools.PrintException(e);
            }
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            Crewmates();
            gun.PreventNormalFireAudio = true;

        }
        private bool HasReloaded;

        protected override void Update()
        {
            if (gun.CurrentOwner)
            {

                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;

                }

                bool flag = base.Owner && !this.HatObject && base.Owner.CurrentGun.sprite;
                if (flag)
                {
                    this.SpawnVFXAttached();



                }
                bool flag2 = !base.Owner.CurrentGun.sprite && this.HatObject;
                if (flag2)
                {
                    UnityEngine.Object.Destroy(this.HatObject);
                }
            }

        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);



            }
        }


        public System.Random rng = new System.Random();
        public void Crewmates()
        {

            int crewtype = rng.Next(0, 40);
            if (crewtype < 13)
            {
                int type = laugh.Next(1, 2);
                if (type == 1)
                {
                    AkSoundEngine.PostEvent("Play_AGUNIM_VO_FIGHT_LAUGH_02", gameObject);
                }
                else
                {
                    AkSoundEngine.PostEvent("Play_AGUNIM_VO_FIGHT_LAUGH_01", gameObject);
                }

                PlayerController owner = this.gun.CurrentOwner as PlayerController;
                AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid((string)guid.GetValue(crewtype));
                IntVector2? intVector = new IntVector2?(owner.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
                AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Default, true);
                aiactor.CanTargetEnemies = true;
                aiactor.CanTargetPlayers = false;
                PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);
                aiactor.gameObject.AddComponent<KillOnRoomClear>();
                aiactor.IsHarmlessEnemy = true;
                aiactor.BaseMovementSpeed = 8;
                aiactor.IgnoreForRoomClear = true;
                aiactor.HandleReinforcementFallIntoRoom(0f);
            }
            else
            {
                PlayerController player = (PlayerController)this.gun.CurrentOwner;

                if (player.PlayerHasActiveSynergy("Sharkbait hoo ha ha!"))
                {
                    Projectile shatterer = ((Gun)ETGMod.Databases.Items[359]).DefaultModule.chargeProjectiles[0].Projectile;
                    GameObject gameObject = SpawnManager.SpawnProjectile(shatterer.gameObject, player.specRigidbody.UnitCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle));
                    Projectile component = gameObject.GetComponent<Projectile>();
                    component.Owner = player;


                }
            }
        }

        public static void BuildPrefab()
        {   //hatdoer
            GameObject gameObject = SpriteBuilder.SpriteFromResource("knifeJam/Resources/beard_collection/beard_right.png", null);
            gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(gameObject);
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            GameObject gameObject2 = new GameObject("beard");
            tk2dSprite tk2dSprite = gameObject2.AddComponent<tk2dSprite>();
            tk2dSprite.SetSprite(gameObject.GetComponent<tk2dBaseSprite>().Collection, gameObject.GetComponent<tk2dBaseSprite>().spriteId);
            Beard.spriteIds.Add(SpriteBuilder.AddSpriteToCollection("knifeJam/Resources/beard_collection/beard_left", tk2dSprite.Collection));
            Beard.spriteIds.Add(SpriteBuilder.AddSpriteToCollection("knifeJam/Resources/beard_collection/beard_up", tk2dSprite.Collection));

            tk2dSprite.GetCurrentSpriteDef().material.shader = ShaderCache.Acquire("Brave/PlayerShader");
            Beard.spriteIds.Add(tk2dSprite.spriteId);
            gameObject2.SetActive(false);
            tk2dSprite.SetSprite(Beard.spriteIds[0]);
            tk2dSprite.GetCurrentSpriteDef().material.shader = ShaderCache.Acquire("Brave/PlayerShader");
            tk2dSprite.SetSprite(Beard.spriteIds[1]);
            tk2dSprite.GetCurrentSpriteDef().material.shader = ShaderCache.Acquire("Brave/PlayerShader");
            tk2dSprite.SetSprite(Beard.spriteIds[2]);
            tk2dSprite.GetCurrentSpriteDef().material.shader = ShaderCache.Acquire("Brave/PlayerShader");

            FakePrefab.MarkAsFakePrefab(gameObject2);
            UnityEngine.Object.DontDestroyOnLoad(gameObject2);
            Beard.skullprefab = gameObject2;
        }

        // Token: 0x06000064 RID: 100 RVA: 0x00004E20 File Offset: 0x00003020
        private void SpawnVFXAttached()
        {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Beard.skullprefab, base.Owner.transform.position + new Vector3(0.6f, 1.05f, -5f), Quaternion.identity);
            gameObject.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(base.Owner.specRigidbody.UnitCenter, tk2dBaseSprite.Anchor.MiddleCenter);
            GameManager.Instance.StartCoroutine(this.HandleSprite(gameObject));
            this.HatObject = gameObject;
        }


        public IEnumerator HandleSprite(GameObject prefab)
        {
            PlayerController owner = this.gun.CurrentOwner as PlayerController;
            while (prefab != null && base.Owner != null)
            {
                prefab.transform.position = base.Owner.transform.position + new Vector3(.2f, 1.1f, -5f);
                bool isFalling = base.Owner.IsFalling;
                if (isFalling)
                {
                    prefab.GetComponent<tk2dBaseSprite>().renderer.enabled = false;
                }
                else
                {
                    prefab.GetComponent<tk2dBaseSprite>().renderer.enabled = true;
                }
                bool flag = owner.IsBackfacing();
                if (flag)
                {
                    prefab.GetComponent<tk2dBaseSprite>().SetSprite(Beard.spriteIds[1]);
                }
                bool flag2 = !owner.IsBackfacing() && owner.CurrentGun.sprite.WorldCenter.x - owner.specRigidbody.UnitCenter.x < 0f;
                if (flag2)
                {
                    prefab.GetComponent<tk2dBaseSprite>().SetSprite(Beard.spriteIds[0]);
                }
                bool flag3 = !owner.IsBackfacing() && owner.CurrentGun.sprite.WorldCenter.x - owner.specRigidbody.UnitCenter.x > 0f;
                if (flag3)
                {
                    prefab.GetComponent<tk2dBaseSprite>().SetSprite(Beard.spriteIds[2]);
                }


                yield return null;
            }
            UnityEngine.Object.Destroy(prefab.gameObject);
            yield break;
        }


        private static GameObject skullprefab;

        private GameObject HatObject;


        public static List<int> spriteIds = new List<int>();



        public string[] guid =
        {
            "c0260c286c8d4538a697c5bf24976ccf", //bomber
            "6f818f482a5c47fd8f38cce101f6566c",
            "6f818f482a5c47fd8f38cce101f6566c",
            "6f818f482a5c47fd8f38cce101f6566c",
            "6f818f482a5c47fd8f38cce101f6566c",
            "6f818f482a5c47fd8f38cce101f6566c",
            "6f818f482a5c47fd8f38cce101f6566c" ,//pirate
            "4b21a913e8c54056bc05cafecf9da880", //parrot
            "86dfc13486ee4f559189de53cfb84107",
            "86dfc13486ee4f559189de53cfb84107",
            "86dfc13486ee4f559189de53cfb84107",
            "86dfc13486ee4f559189de53cfb84107",//pirate sh
            "c0260c286c8d4538a697c5bf24976ccf", //
            "72d2f44431da43b8a3bae7d8a114a46d"
        };


    }
}

