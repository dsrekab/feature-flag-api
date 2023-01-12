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
        public async Task FeatureIsEnabled_ReturnsTrue_WhenEnabledForAllIsTrue()
        {
            _mockFeatureFlagRepository.Setup(m => m.GetFeatureFlag("UnitTestService", "EnabledFlag"))
                .ReturnsAsync(new FeatureFlagRepoItem
                {
                    ServiceName = "UnitTestService",
                    FlagName = "EnabledFlag",
                    Enabled = true
                });

            var actual = await _sut.FeatureIsEnabled("UnitTestService", "EnabledFlag");

            actual.Should().BeTrue();
        }

        [Fact]
        public async Task FeatureIsEnabled_ReturnsFalse_WhenEnabledIsFalse()
        {
            _mockFeatureFlagRepository.Setup(m => m.GetFeatureFlag("UnitTestService", "FlagNullEnabledFor"))
                .ReturnsAsync(new FeatureFlagRepoItem
                {
                    ServiceName = "UnitTestService",
                    FlagName = "FlagNullEnabledFor",
                    Enabled = false
                });

            var actual = await _sut.FeatureIsEnabled("UnitTestService", "FlagNullEnabledFor");

            actual.Should().BeFalse();
        }

        [Fact]
        public async Task FeatureIsEnabled_ReturnsFalse_WhenFlagIsNull()
        {
            _mockFeatureFlagRepository.Setup(m => m.GetFeatureFlag("UnitTestService", "FlagNullEnabledFor"))
                .ReturnsAsync((FeatureFlagRepoItem)null);

            var actual = await _sut.FeatureIsEnabled("UnitTestService", "FlagNullEnabledFor");

            actual.Should().BeFalse();
        }

        [Fact]
        public async Task FeatureIsEnabled_ReturnsFalse_WhenFlagIsEmpty()
        {
            _mockFeatureFlagRepository.Setup(m => m.GetFeatureFlag("UnitTestService", "FlagNullEnabledFor"))
                .ReturnsAsync(new FeatureFlagRepoItem());

            var actual = await _sut.FeatureIsEnabled("UnitTestService", "FlagNullEnabledFor");

            actual.Should().BeFalse();
        }
    }
}