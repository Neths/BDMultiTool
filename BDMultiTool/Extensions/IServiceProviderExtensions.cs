using System;

namespace BDMultiTool.Extensions
{
    public static class IServiceProviderExtensions
    {
        public static T GetInstance<T>(this IServiceProvider serviceProvider)
        {
            return (T) serviceProvider.GetService(typeof(T));
        }
    }
}
