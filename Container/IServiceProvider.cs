namespace Sikor.Container
{
    /**
     * <summary>
     * Service provider class: provides access to shared services.
     * </summary>
     */
    public interface IServiceProvider
    {
        /**
         * <summary>
         * Registers a context provider or service.
         * </summary>
         */
        void Register();

        /**
         * <summary>
         * Initializes a context provider or service.
         * </summary>
         */
        void Init();

        /**
         * <summary>
         * Called whenever a manual initialization is required post any other
         * init operations: for example when we need to load data from storage.
         * </summary>
         */
        void PostInit();

        /**
         * <summary>
         * Returns the type as string.
         * </summary>
         * <returns></returns>
         */
        string GetTypeString();
    }
}
