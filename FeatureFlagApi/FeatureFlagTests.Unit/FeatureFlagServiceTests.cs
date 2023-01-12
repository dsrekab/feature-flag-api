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
        public async Task FeatureIsEnabled_ReturnsTrue_WhenEnabledIsTrue()
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
            _mockFeatureFlagRepository.Setup(m => m.GetFeatureFlag("UnitTestService", "DisabledFlag"))
                .ReturnsAsync(new FeatureFlagRepoItem
                {
                    ServiceName = "UnitTestService",
                    FlagName = "DisabledFlag",
                    Enabled = false
                });

            var actual = await _sut.FeatureIsEnabled("UnitTestService", "DisabledFlag");

            actual.Should().BeFalse();
        }

        [Fact]
        public async Task FeatureIsEnabled_ReturnsFalse_WhenFlagIsNull()
        {
            _mockFeatureFlagRepository.Setup(m => m.GetFeatureFlag("UnitTestService", "nullFlag"))
                .ReturnsAsync((FeatureFlagRepoItem)null);

            var actual = await _sut.FeatureIsEnabled("UnitTestService", "nullFlag");

            actual.Should().BeFalse();
        }

        [Fact]
        public async Task FeatureIsEnabled_ReturnsFalse_WhenFlagIsEmpty()
        {
            _mockFeatureFlagRepository.Setup(m => m.GetFeatureFlag("UnitTestService", "MissingFlag"))
                .ReturnsAsync(new FeatureFlagRepoItem());

            var actual = await _sut.FeatureIsEnabled("UnitTestService", "MissingFlag");

            actual.Should().BeFalse();
        }

        [Fact]
        public async Task GetFeatureFlag_ReturnsTrueFlag_WhenEnabledIsTrue()
        {
            _mockFeatureFlagRepository.Setup(m => m.GetFeatureFlag("UnitTestService", "EnabledFlag"))
                .ReturnsAsync(new FeatureFlagRepoItem
                {
                    ServiceName = "UnitTestService",
                    FlagName = "EnabledFlag",
                    Enabled = true
                });

            var actual = await _sut.GetFeatureFlag("UnitTestService", "EnabledFlag");

            actual.ServiceName.Should().BeEquivalentTo("UnitTestService");
            actual.FlagName.Should().BeEquivalentTo("EnabledFlag");
            actual.Enabled.Should().BeTrue();
        }

        [Fact]
        public async Task GetFeatureFlag_ReturnsFalseFlag_WhenEnabledIsFalse()
        {
            _mockFeatureFlagRepository.Setup(m => m.GetFeatureFlag("UnitTestService", "DisabledFlag"))
                .ReturnsAsync(new FeatureFlagRepoItem
                {
                    ServiceName = "UnitTestService",
                    FlagName = "DisabledFlag",
                    Enabled = false
                });

            var actual = await _sut.GetFeatureFlag("UnitTestService", "DisabledFlag");

            actual.ServiceName.Should().BeEquivalentTo("UnitTestService");
            actual.FlagName.Should().BeEquivalentTo("DisabledFlag");
            actual.Enabled.Should().BeFalse();
        }

        [Fact]
        public async Task GetFeatureFlag_ReturnsFalseFlag_WhenFlagIsMissing()
        {
            _mockFeatureFlagRepository.Setup(m => m.GetFeatureFlag("UnitTestService", "MissingFlag"))
                .ReturnsAsync((FeatureFlagRepoItem)null);

            var actual = await _sut.GetFeatureFlag("UnitTestService", "MissingFlag");

            actual.ServiceName.Should().BeEquivalentTo("UnitTestService");
            actual.FlagName.Should().BeEquivalentTo("MissingFlag");
            actual.Enabled.Should().BeFalse();
        }

        [Fact]
        public async Task GetAllFeatureFlagsByService_ReturnsListOfFlags()
        {
            _mockFeatureFlagRepository.Setup(m => m.GetAllFeatureFlagsByService("UnitTestService"))
                .ReturnsAsync(new List<FeatureFlagRepoItem>
                {
                    new FeatureFlagRepoItem{ServiceName="UnitTestService",FlagName="flag1",Enabled=true},
                    new FeatureFlagRepoItem{ServiceName="UnitTestService",FlagName="flag2",Enabled=false},
                    new FeatureFlagRepoItem{ServiceName="UnitTestService",FlagName="flag3",Enabled=true}
                });

            var actual = await _sut.GetAllFeatureFlagsByService("UnitTestService");

            actual.Count.Should().Be(3);

            actual.Where(f => f.FlagName == "flag1").FirstOrDefault().Enabled.Should().BeTrue();
            actual.Where(f => f.FlagName == "flag2").FirstOrDefault().Enabled.Should().BeFalse();
            actual.Where(f => f.FlagName == "flag3").FirstOrDefault().Enabled.Should().BeTrue();
        }
    }
}