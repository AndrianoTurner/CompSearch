
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using PulsarModLoader;
using UnityEngine;
using System.Reflection;
namespace CompSearch
{

    internal class ShipComponentPatch
    {
        
  
        [HarmonyPatch]
        internal class ConstructorPatch
        {
            static IEnumerable<MethodBase> TargetMethods()
            {
             

               return AccessTools.GetTypesFromAssembly(typeof(PLServer).Assembly)
                .Where(t => typeof(PLShipComponent).IsAssignableFrom(t) || typeof(PLPawnItem).IsAssignableFrom(t))
                 .Where(t => t != typeof(PLShipComponent) && t != typeof(PLPoweredShipComponent))
                .SelectMany(t => t.GetConstructors())
                .Cast<MethodBase>();
            }
            public static void Postfix(MethodBase __originalMethod, PLWare __instance)
            {
                if (__instance is PLShipComponent)
                {
                    __instance.Desc += String.Format("\nGX: {0:d2}-{1:d2}-{2:d2}", (int)((PLShipComponent)__instance).ActualSlotType, ((PLShipComponent)__instance).SubType, ((PLShipComponent)__instance).Level);
                }
                else
                {
                    __instance.Desc += String.Format("\nGX: {0:d2}-{1:d2}-{2:d2}", (int)((PLPawnItem)__instance).PawnItemType, ((PLPawnItem)__instance).SubType, ((PLPawnItem)__instance).Level);
                }
              
                
                
            }

            
        }



     
      
    }

    
}
