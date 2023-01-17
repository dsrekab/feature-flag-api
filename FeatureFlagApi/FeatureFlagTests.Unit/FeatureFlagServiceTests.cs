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
            _mockFeatureFlagRepository.Setup(m => m.GetFeatureFlags("UnitTestService", "EnabledFlag"))
                .ReturnsAsync(new List<FeatureFlagRepoItem>{ new FeatureFlagRepoItem
                {
                    ServiceName = "UnitTestService",
                    FlagName = "EnabledFlag",
                    Enabled = true
                } });

            var actual = await _sut.FeatureIsEnabled("UnitTestService", "EnabledFlag");

            actual.Should().BeTrue();
        }

        [Fact]
        public async Task FeatureIsEnabled_ReturnsFalse_WhenEnabledIsFalse()
        {
            _mockFeatureFlagRepository.Setup(m => m.GetFeatureFlags("UnitTestService", "DisabledFlag"))
                .ReturnsAsync(new List<FeatureFlagRepoItem>{ new FeatureFlagRepoItem
                {
                    ServiceName = "UnitTestService",
                    FlagName = "DisabledFlag",
                    Enabled = false
                } });

            var actual = await _sut.FeatureIsEnabled("UnitTestService", "DisabledFlag");

            actual.Should().BeFalse();
        }

        [Fact]
        public async Task FeatureIsEnabled_ReturnsFalse_WhenFlagIsNull()
        {
            _mockFeatureFlagRepository.Setup(m => m.GetFeatureFlags("UnitTestService", "nullFlag"))
                .ReturnsAsync(new List<FeatureFlagRepoItem>());

            var actual = await _sut.FeatureIsEnabled("UnitTestService", "nullFlag");

            actual.Should().BeFalse();
        }

        [Fact]
        public async Task FeatureIsEnabled_ReturnsFalse_WhenFlagIsEmpty()
        {
            _mockFeatureFlagRepository.Setup(m => m.GetFeatureFlags("UnitTestService", "MissingFlag"))
                .ReturnsAsync(new List<FeatureFlagRepoItem> { new FeatureFlagRepoItem() });

            var actual = await _sut.FeatureIsEnabled("UnitTestService", "MissingFlag");

            actual.Should().BeFalse();
        }

        [Fact]
        public async Task GetFeatureFlags_ReturnsTrueFlag_WhenEnabledIsTrue()
        {
            _mockFeatureFlagRepository.Setup(m => m.GetFeatureFlags("UnitTestService", "EnabledFlag"))
                .ReturnsAsync(new List<FeatureFlagRepoItem>{ new FeatureFlagRepoItem
                {
                    ServiceName = "UnitTestService",
                    FlagName = "EnabledFlag",
                    Enabled = true
                } });

            var actual = (await _sut.GetFeatureFlags("UnitTestService", "EnabledFlag")).FirstOrDefault();

            actual.ServiceName.Should().BeEquivalentTo("UnitTestService");
            actual.FlagName.Should().BeEquivalentTo("EnabledFlag");
            actual.Enabled.Should().BeTrue();
        }

        [Fact]
        public async Task GetFeatureFlags_ReturnsFalseFlag_WhenEnabledIsFalse()
        {
            _mockFeatureFlagRepository.Setup(m => m.GetFeatureFlags("UnitTestService", "DisabledFlag"))
                .ReturnsAsync(new List<FeatureFlagRepoItem>{ new FeatureFlagRepoItem
                {
                    ServiceName = "UnitTestService",
                    FlagName = "DisabledFlag",
                    Enabled = false
                } });

            var actual = (await _sut.GetFeatureFlags("UnitTestService", "DisabledFlag")).FirstOrDefault();

            actual.ServiceName.Should().BeEquivalentTo("UnitTestService");
            actual.FlagName.Should().BeEquivalentTo("DisabledFlag");
            actual.Enabled.Should().BeFalse();
        }

        [Fact]
        public async Task GetFeatureFlags_ReturnsFalseFlag_WhenFlagIsMissing()
        {
            _mockFeatureFlagRepository.Setup(m => m.GetFeatureFlags("UnitTestService", "MissingFlag"))
                .ReturnsAsync(new List<FeatureFlagRepoItem>());

            var actual = (await _sut.GetFeatureFlags("UnitTestService", "MissingFlag")).FirstOrDefault();

            actual.ServiceName.Should().BeEquivalentTo("UnitTestService");
            actual.FlagName.Should().BeEquivalentTo("MissingFlag");
            actual.Enabled.Should().BeFalse();
        }

        [Fact]
        public async Task CreateFeatureFlag_ThrowsException_WhenTryingToCreateAnExistingFlag()
        {
            var testFlag = new FeatureFlag { ServiceName = "testService", FlagName = "existingFlag" };

            _mockFeatureFlagRepository.Setup(m => m.GetFeatureFlags("testService", "existingFlag"))
                .ReturnsAsync(new List<FeatureFlagRepoItem> { new FeatureFlagRepoItem { ServiceName = "testService", FlagName = "existingFlag", Enabled = true, LastUpdated = DateTimeOffset.Now } });

            await _sut.Invoking(f => f.CreateFeatureFlag(testFlag))
                .Should().ThrowAsync<Exception>()
                .WithMessage("FeatureFlag existingFlag in Service testService already exists");
        }

        [Fact]
        public async Task CreateFeatureFlag_ThrowsException_WhenCreatingFlagFails()
        {
            var testFlag = new FeatureFlag { ServiceName = "testService", FlagName = "failingFlag" };

            _mockFeatureFlagRepository.Setup(m => m.GetFeatureFlags("testService", "failingFlag"))
                .ReturnsAsync(new List<FeatureFlagRepoItem> { new FeatureFlagRepoItem() });

            _mockFeatureFlagRepository.Setup(m => m.CreateFeatureFlag(testFlag))
                .ReturnsAsync(new FeatureFlagRepoItem());

            await _sut.Invoking(f => f.CreateFeatureFlag(testFlag))
                .Should().ThrowAsync<Exception>()
                .WithMessage("Unable to create Feature Flag failingFlag in Service testService");
        }

        [Fact]
        public async Task CreateFeatureFlag_ReturnsCreatedFeatureFlag_WhenCreated()
        {
            var testFlag = new FeatureFlag { ServiceName = "testService", FlagName = "newFlag" };

            _mockFeatureFlagRepository.Setup(m => m.GetFeatureFlags("testService", "newFlag"))
                .ReturnsAsync(new List<FeatureFlagRepoItem> { new FeatureFlagRepoItem() });

            _mockFeatureFlagRepository.Setup(m => m.CreateFeatureFlag(testFlag))
                .ReturnsAsync(new FeatureFlagRepoItem { ServiceName = "testService", FlagName = "newFlag", Enabled = true, LastUpdated = DateTimeOffset.Now });

            var actual = await _sut.CreateFeatureFlag(testFlag);

            actual.Should().NotBeNull();
            actual.ServiceName.Should().BeEquivalentTo("testService");
            actual.FlagName.Should().BeEquivalentTo("newFlag");
            actual.Enabled.Should().BeTrue();
        }


    }
}