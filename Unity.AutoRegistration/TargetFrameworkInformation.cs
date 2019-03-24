using System;

[assembly: Unity.AutoRegistration.TargetFrameworkInformation(
#if NETSTANDARD2_0
    Unity.AutoRegistration.TargetFramework.netstandard2_0
#elif NET461
    Unity.AutoRegistration.TargetFramework.net461
#endif
    )]
namespace Unity.AutoRegistration
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
