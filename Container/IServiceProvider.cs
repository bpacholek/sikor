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
        public void Register();

        /**
         * <summary>
         * Initializes a context provider or service.
         * </summary>
         */
        public void Init();

        public string GetTypeString();
    }
}
