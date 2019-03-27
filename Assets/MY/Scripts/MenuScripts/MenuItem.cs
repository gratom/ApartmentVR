using UnityEngine;

namespace MyVRMenu
{

    /// <summary>
    /// Class for menu object. All menu items are such classes
    /// </summary>
    public class MenuItem : InteractiveObject
    {
        /// <summary>
        /// Size of side of cube in which MenuItem will be inscribed
        /// </summary>
        public float MenuItemSize = 0.085f;

        public float CurrentRotation;

        public MenuLine.TypeOfLine typeOfObject;

        public MonoBehaviour AttachedObject;

        public GameObject effectGameObject;

        public TextMesh VisibleName;

        public Animator animator;

        #region Delegates

        /// <summary>
        /// Delegate, that should called, when the item will drow by MenuManager
        /// </summary>
        public SimpleActionDelegate onDrawFunction
        {
            get
            {
                if (_onDrawFunction == null)
                {
                    _onDrawFunction = () => { };
                }
                return _onDrawFunction;
            }
            set
            {
                _onDrawFunction = value;
            }
        }
        private SimpleActionDelegate _onDrawFunction;

        #endregion

        public void HideElement()
        {
            gameObject.SetActive(false);
        }

        public void ShowElement()
        {
            gameObject.SetActive(true);
        }

    }

}