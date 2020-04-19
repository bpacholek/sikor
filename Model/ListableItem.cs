using System;

namespace Sikor.Model
{
    /**
     * <summary>
     * Model of a basic entity which can be displayed in a selectable list.
     * </summary>
     */
    [Serializable]
    public class ListableItem
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

        public string Name
        {
            get
            {
                return this.Value;
            }
        }

        public override string ToString()
        {
            return this.Value;
        }

    }
}
