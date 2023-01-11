using FeatureFlagApi.Models;
using FeatureFlagApi.Repositories.Interfaces;
using FeatureFlagApi.Services;
using FluentAssertions;
using Moq;

namespace FeatureFlagTests.Unit
{
    public class FeatureFlagServiceTests
    {
        private readonly Mock<IFeatureFlagRepository> _mockFeatureFlagRepository;
        private readonly FeatureFlagService _sut;

        public FeatureFlagServiceTests()
        {
            _mockFeatureFlagRepository = new Mock<IFeatureFlagRepository>();
            _sut = new FeatureFlagService(_mockFeatureFlagRepository.Object);
        }

        [Fact]
        public void FeatureIsEnabled_ReturnsTrue_WhenEnabledForAllIsTrue()
        {
            _mockFeatureFlagRepository.Setup(m => m.GetFeatureFlag("UnitTestService", "EnabledFlag"))
                .Returns(new FeatureFlag
                {
                    ServiceName = "UnitTestService",
                    FlagName = "EnabledFlag",
                    EnabledFor = new EnabledFor
                    {
                        All = true
                    }
                });

            var actual = _sut.FeatureIsEnabled("UnitTestService", "EnabledFlag");

            actual.Should().BeTrue();
        }

        [Fact]
        public void FeatureIsEnabled_ReturnsFalse_WhenEnabledForAllIsNull()
        {
            _mockFeatureFlagRepository.Setup(m => m.GetFeatureFlag("UnitTestService", "FlagNullEnabledFor"))
                .Returns(new FeatureFlag
                {
                    ServiceName = "UnitTestService",
                    FlagName = "FlagNullEnabledFor",
                    EnabledFor = null
                });

            var actual = _sut.FeatureIsEnabled("UnitTestService", "FlagNullEnabledFor");

            actual.Should().BeFalse();
        }

        [Fact]
        public void FeatureIsEnabled_ReturnsFalse_WhenEnabledForAllIsEmpty()
        {
            _mockFeatureFlagRepository.Setup(m => m.GetFeatureFlag("UnitTestService", "FlagMissingEnabledFor"))
                .Returns(new FeatureFlag
                {
                    ServiceName = "UnitTestService",
                    FlagName = "FlagMissingEnabledFor",
                    EnabledFor = new EnabledFor()
                });

            var actual = _sut.FeatureIsEnabled("UnitTestService", "FlagMissingEnabledFor");

            actual.Should().BeFalse();
        }

        [Fact]
        public void FeatureIsEnabled_ReturnsFalse_WhenEnabledForAllIsFalse_AndRolloutIsNull()
        {
            _mockFeatureFlagRepository.Setup(m => m.GetFeatureFlag("UnitTestService", "FlagNullRollout"))
                .Returns(new FeatureFlag
                {
                    ServiceName = "UnitTestService",
                    FlagName = "FlagNullRollout",
                    EnabledFor = new EnabledFor
                    {
                        All = false,
                        Rollout = null
                    }
                });

            var actual = _sut.FeatureIsEnabled("UnitTestService", "FlagNullRollout");

            actual.Should().BeFalse();
        }

        [Fact]
        public void FeatureIsEnabled_ReturnsFalse_WhenEnabledForAllIsFalse_AndRolloutIsEmpty()
        {
            _mockFeatureFlagRepository.Setup(m => m.GetFeatureFlag("UnitTestService", "FlagMissingRollout"))
                .Returns(new FeatureFlag
                {
                    ServiceName = "UnitTestService",
                    FlagName = "FlagMissingRollout",
                    EnabledFor = new EnabledFor
                    {
                        All = false,
                        Rollout = new Rollout()
                    }
                });

            var actual = _sut.FeatureIsEnabled("UnitTestService", "FlagMissingRollout");

            actual.Should().BeFalse();
        }
    }
}