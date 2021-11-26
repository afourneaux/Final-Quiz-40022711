using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Runtime
{
    /// <summary>
    /// Small script to reference the owner of this collider. 
    /// Sometimes colliders are placed on objects other than the ones which have the script that you are looking for. 
    /// This script allows you to easily refer to the correct object on collision. You just need to properly set it up in the editor.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ColliderOwner : MonoBehaviour
    {
        // The "owner" gameobject
        public GameObject owner;
    }

    /// <summary>
    /// Extends Unity's Collider class and adds helper method to get the gameobject owner directly from collider;
    /// </summary>
    public static class ColliderExt
    {
        public static GameObject GetOwner(this Collider collider)
        {
            ColliderOwner colliderOwner = collider.GetComponent<ColliderOwner>();
            return colliderOwner == null || colliderOwner.owner == null ? collider.gameObject : colliderOwner.owner;
        }
    }
}