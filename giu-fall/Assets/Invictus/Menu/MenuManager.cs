using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MenuGUI
{
    public class MenuManager : MonoBehaviour
    {

        class StackItem
        {
            internal Menu menu;
            internal ActivateParams activateParams;
        }


        static MenuManager instance;
        public static MenuManager Instance { get { return instance; } }

        [SerializeField] int startMenu;
        [SerializeField, ResourcePath(typeof(Menu))]
        string[] preloadedMenus;



        Dictionary<int, Menu> menus = new Dictionary<int, Menu>();
        Stack<StackItem> menuStack = new Stack<StackItem>();


        void CreateMenuFromAsset(string path)
        {
            Menu prefab = Resources.Load<Menu>(path);
            Menu menu = Instantiate(prefab) as Menu;
            menu.transform.SetParent(transform);
            menu.gameObject.SetActive(false);
        }


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                // A common menüket hozzáadjuk
                foreach (string path in preloadedMenus)
                    CreateMenuFromAsset(path);
                Menu[] menusArray = GetComponentsInChildren<Menu>(true);
                foreach (Menu menu in menusArray)
                {
                    menus.Add(menu.UniqueID, menu);
                    // Ha bekapcsolva maradt a prefabban akkor kikapcsoljuk
                    menu.gameObject.SetActive(false);
                }
            }
            else
                Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                menus.Clear();
                menuStack.Clear();
                instance = null;
            }
        }

        private void Start()
        {
            if (startMenu >= 0)
                Show(startMenu);

            //显示隐私协议
			//if(PlayerPrefs.GetInt("GDPR_" + "gdpr" + "_enable") == 0)
			//GDPR.Instance.ShowPopup("gdpr", "用户定制广告显示", "为了改进此应用，并向您展示相关广告，《永不言弃·掉落》将收集用户数据和设备信息，并与合作伙伴共享。使用本应用即表示您同意本应用的服务条款和隐私政策。");
        }

        public Menu CurrentMenu { get { return menuStack.Count > 0 ? menuStack.Peek().menu : null; } }

        public void Show(int id, ActivateParams activateParams = null)
        {
            Menu menu;
            if (menus.TryGetValue(id, out menu))
            {
                if (menu.IsPopup)
                    menu = Instantiate(menu.gameObject, menu.transform.parent).GetComponent<Menu>();
                Menu prevMenu = (menuStack.Count > 0) ? menuStack.Peek().menu : null;
                menuStack.Push(new StackItem() { menu = menu, activateParams = activateParams });

                if (prevMenu != null && menu != prevMenu && !menu.IsPopup)
                {
                    prevMenu.Deactivate(() =>
                    {
                        if (!menu.IsActive)
                            menu.Activate(activateParams);
                    });
                }else if (!menu.IsActive)
                    menu.Activate(activateParams);
            }
            else
                throw new UnassignedReferenceException(string.Format("Menu was not found with this name:{0}", id));

            
            //PluginMercury.Instance.ActiveBanner();
        }
        public void Back()
        {
            if (menuStack.Count > 1)
            {
                Menu menu = menuStack.Pop().menu;
                menu.Deactivate(() =>
                {
                    if (menu.IsPopup)
                        Destroy(menu.gameObject);
                    StackItem top = menuStack.Peek();
                    if (!top.menu.IsActive)
                        top.menu.Activate(top.activateParams);
                });
            }
        }

        public Menu GetMenu(int type)
        {
            return menus[type];
        }
    }
}