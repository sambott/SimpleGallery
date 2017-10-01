using System.Diagnostics;
using Xunit;

namespace SimpleGallery.Core.Tests
{
    public sealed class IntegrationFactAttribute : FactAttribute
    {
        public IntegrationFactAttribute()
        {
            if (!Debugger.IsAttached)
            {
                Skip = "Only running integration tests interactively.";
            }
        }
    }
}