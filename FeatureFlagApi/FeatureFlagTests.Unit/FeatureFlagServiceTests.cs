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
                    Enabled = true
                });

            var actual = _sut.FeatureIsEnabled("UnitTestService", "EnabledFlag");

            actual.Should().BeTrue();
        }

        [Fact]
        public void FeatureIsEnabled_ReturnsFalse_WhenEnabledIsFalse()
        {
            _mockFeatureFlagRepository.Setup(m => m.GetFeatureFlag("UnitTestService", "FlagNullEnabledFor"))
                .Returns(new FeatureFlag
                {
                    ServiceName = "UnitTestService",
                    FlagName = "FlagNullEnabledFor",
                    Enabled = false
                });

            var actual = _sut.FeatureIsEnabled("UnitTestService", "FlagNullEnabledFor");

            actual.Should().BeFalse();
        }

        [Fact]
        public void FeatureIsEnabled_ReturnsFalse_WhenEnabledIsNull()
        {
            _mockFeatureFlagRepository.Setup(m => m.GetFeatureFlag("UnitTestService", "FlagNullEnabledFor"))
                .Returns(new FeatureFlag
                {
                    ServiceName = "UnitTestService",
                    FlagName = "FlagNullEnabledFor",
                    Enabled = null
                });

            var actual = _sut.FeatureIsEnabled("UnitTestService", "FlagNullEnabledFor");

            actual.Should().BeFalse();
        }

        [Fact]
        public void FeatureIsEnabled_ReturnsFalse_WhenFlagIsNull()
        {
            _mockFeatureFlagRepository.Setup(m => m.GetFeatureFlag("UnitTestService", "FlagNullEnabledFor"))
                .Returns((FeatureFlag)null);

            var actual = _sut.FeatureIsEnabled("UnitTestService", "FlagNullEnabledFor");

            actual.Should().BeFalse();
        }

        [Fact]
        public void FeatureIsEnabled_ReturnsFalse_WhenFlagIsEmpty()
        {
            _mockFeatureFlagRepository.Setup(m => m.GetFeatureFlag("UnitTestService", "FlagNullEnabledFor"))
                .Returns(new FeatureFlag());

            var actual = _sut.FeatureIsEnabled("UnitTestService", "FlagNullEnabledFor");

            actual.Should().BeFalse();
        }
    }
}