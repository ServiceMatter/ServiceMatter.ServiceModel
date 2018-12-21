namespace ServiceMatter.ServiceModel.Configuration
{

    public static class ServiceModelConfiguration<TAmbientContext>
        where TAmbientContext : class
    {
        public static ProxyFactoryConfiguration<TAmbientContext> ProxyConfiguration { get; } = new ProxyFactoryConfiguration<TAmbientContext>();

    }

}
