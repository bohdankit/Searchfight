using AutoFixture.Xunit2;

namespace Searchfight.TestHelpers
{
    public class InlineAutoMoqDataAttribute : InlineAutoDataAttribute
    {
        public InlineAutoMoqDataAttribute(params object[] objects)
            : base(new AutoMoqDataAttribute(), objects)
        {
        }
    }
}
