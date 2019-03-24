using System;

[assembly: Microsoft.Extensions.DependencyInjection.AutoRegistration.TargetFrameworkInformation(
#if NETSTANDARD2_0
    Microsoft.Extensions.DependencyInjection.AutoRegistration.TargetFramework.netstandard2_0
#elif NET461
    Microsoft.Extensions.DependencyInjection.AutoRegistration.TargetFramework.net461
#endif
    )]
namespace Microsoft.Extensions.DependencyInjection.AutoRegistration
{

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public enum TargetFramework
    {
        Unknown,
        netstandard2_0,
        net461
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [AttributeUsage(AttributeTargets.Assembly)]
    public class TargetFrameworkInformationAttribute : Attribute
    {

        public TargetFramework TargetFramework { get; }

        public TargetFrameworkInformationAttribute(TargetFramework targetFramework)
        {
            TargetFramework = targetFramework;
        }
    }
}
