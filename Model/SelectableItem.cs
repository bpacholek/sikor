using System;

namespace Sikor.Model
{
    /**
     * <summary>
     * Represents an list item which can hold "selected" attribute.
     * </summary>
     */
    [Serializable]
    abstract public class SelectableItem : ListableItem
    {
        /**
         * <summary>
         * True if item is selected on the list. Built using a separate attribute
         * in order to allow execution of special methods on selection.
         * </summary>
         */
        public bool selected;
        public bool Selected {
            get => selected;
            set
            {
                //allows to override selection in child classes
                value = PreChange(value);
                selected = value;
                PostChange();
            }
        }

        /**
         * <summary>
         * Allows to intercept and modify the value of the selection before it
         * is actually stored.
         * </summary>
         * <param name="value">selection value which would be set</param>
         * <returns>selection value</returns>
         */
        public virtual bool PreChange(bool value)
        {
            return value;
        }

        /**
         * <summary>
         * Allows to execute code after the selection has been applied.
         * </summary>
         */
        public virtual void PostChange()
        {
            //do nothing
        }

    }
}
