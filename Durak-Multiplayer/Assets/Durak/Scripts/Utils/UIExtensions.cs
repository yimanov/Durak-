using UnityEngine;
using UnityEngine.EventSystems;

namespace Durak
{
    public static class UIExtensions
    {
        public static void DisableObj(this MonoBehaviour mono)
        {
            mono.gameObject.SetActive(false);
        }

        public static void EnableObj(this MonoBehaviour mono)
        {
            mono.gameObject.SetActive(true);
        }

        public static void Disable(this UIBehaviour ui)
        {
            ui.enabled = false;
        }

        public static void Enable(this UIBehaviour ui)
        {
            ui.enabled = true;
        }
    }
}
