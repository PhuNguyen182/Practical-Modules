using System;

namespace Foundations.Popups.Data
{
    /// <summary>
    /// Base data structure for all popups
    /// </summary>
    [Serializable]
    public class PopupData
    {
        public string id;
        public int priority;
        public bool canCloseOnOutsideClick;
        
        public PopupData()
        {
            id = Guid.NewGuid().ToString();
            priority = 0;
            canCloseOnOutsideClick = true;
        }
        
        public PopupData(string id, int priority = 0, bool canCloseOnOutsideClick = true)
        {
            this.id = id;
            this.priority = priority;
            this.canCloseOnOutsideClick = canCloseOnOutsideClick;
        }
    }
}
