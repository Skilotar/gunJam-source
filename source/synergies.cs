using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;
using JetBrains.Annotations;

namespace knifeJam
{
    class synergiesGJ
    {
        public class sharkbait : AdvancedSynergyEntry
        {

            public sharkbait()
            {
                this.NameKey = "Sharkbait hoo ha ha!";
                this.MandatoryGunIDs = new List<int>
                {

                   PickupObjectDatabase.GetByEncounterName("Cpt Boombeard's Cutlass").PickupObjectId,
                    359,
                    
                };
                this.IgnoreLichEyeBullets = false;
                this.statModifiers = new List<StatModifier>(0)
                {
                
                };

                this.bonusSynergies = new List<CustomSynergyType>();

            }

        }
    }
}
