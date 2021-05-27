using UnityEngine;
using UnityEngine.UI;




public class ActivateParams {

}

namespace MenuGUI
{

    public delegate void ActivatorCallback();

    public interface MenuActivator
    {
        void OnActivate(ActivatorCallback callback);
        void OnDeactivate(ActivatorCallback callback);
    }

    [RequireComponent(typeof(Canvas),typeof(GraphicRaycaster))]
    public abstract class Menu : MonoBehaviour
    {
        [SerializeField] bool _popup = false;

        public bool IsPopup { get { return _popup; } }
        public bool IsActive { get { return gameObject.activeSelf; } }

        internal void Activate(ActivateParams activateParams)
        {
            MenuActivator activator = GetComponent<MenuActivator>();
            if (activator != null)
                activator.OnActivate(() => { gameObject.SetActive(true); OnShow(activateParams); });
            else
            {
                gameObject.SetActive(true);
                OnShow(activateParams);
            }
        }
        internal void Deactivate(ActivatorCallback OnMenuClosed)
        {
            MenuActivator activator = GetComponent<MenuActivator>();
            if (activator != null)
                activator.OnDeactivate(() => { OnClose(); gameObject.SetActive(false); OnMenuClosed?.Invoke(); });
            else
            {
                OnClose();
                gameObject.SetActive(false);
                OnMenuClosed?.Invoke();
            }
        }

        #region
        public abstract int UniqueID { get; }
        protected abstract void OnShow(ActivateParams activateParams);
        protected abstract void OnClose();
        #endregion
    }
}