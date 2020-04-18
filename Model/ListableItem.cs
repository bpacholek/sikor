using System;

namespace Sikor.Model
{
    /**
     * <summary>
     * Model of a basic entity which can be displayed in a selectable list.
     * </summary>
     */
    [Serializable]
    abstract public class ListableItem
    {
        /**
         * <summary>
         * Represents the backend id of an element.
         * </summary>
         * <value>Key, element's id</value>
         */
        public string Key { get; set; }

        /**
         * <summary>
         * Value: the text displayed on the frontend.
         * </summary>
         * <value>Value, textual representation</value>
         */
        public string Value { get; set; }

        public override string ToString()
        {
            return Value;
        }

    }
}
