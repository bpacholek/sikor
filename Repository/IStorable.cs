namespace Sikor.Repository
{
    /**
     * <summary>
     * Interface for entities which can be held in a Repository
     * </summary>
     */
    public interface IStorable
    {
        /**
         * <summary>
         * Returns an unique id of a given entity.
         * </summary>
         * <returns>String, entity's unique id</returns>
         */
        string GetId();
    }
}