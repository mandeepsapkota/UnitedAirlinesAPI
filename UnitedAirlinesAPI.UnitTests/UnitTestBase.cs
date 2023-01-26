using AutoFixture;
using Moq.AutoMock;
using NUnit.Framework;

namespace UnitedAirlinesAPI.UnitTests
{
    public abstract class UnitTestBase
    {
        public Fixture Fixture;
        public AutoMocker Mocker;

        [SetUp]
        public virtual void Setup()
        {
            Fixture = new Fixture();
            Mocker = new AutoMocker();
        }
    }
}
