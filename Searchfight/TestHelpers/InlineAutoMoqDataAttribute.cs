﻿using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace Searchfight.TestHelpers
{
    public class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute()
            : base(() =>
            {
                var fixture = new Fixture().Customize(
                    new CompositeCustomization(
                        new AutoMoqCustomization(),
                        new SupportMutableValueTypesCustomization()));

                fixture.Behaviors.Add(new OmitOnRecursionBehavior());

                return fixture;
            })
        {
        }
    }
}