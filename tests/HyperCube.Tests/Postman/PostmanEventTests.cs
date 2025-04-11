using HyperCube.Postman.Base.Events;

namespace HyperCube.Tests.Postman;

[TestFixture]
public class PostmanEventTests
{
    [Test]
    public void BasePostmanEvent_ShouldInitializeIdAndCreatedAt()
    {
        // Act
        var @event = new TestEvent();

        // Assert
        Assert.That(@event.Id, Is.Not.EqualTo(String.Empty));

    }

    [Test]
    public void MultipleEvents_ShouldHaveUniqueIds()
    {
        // Arrange
        var event1 = new TestEvent();
        var event2 = new TestEvent();
        var event3 = new TestEvent();

        // Assert
        Assert.That(event1.Id, Is.Not.EqualTo(event2.Id));
        Assert.That(event1.Id, Is.Not.EqualTo(event3.Id));
        Assert.That(event2.Id, Is.Not.EqualTo(event3.Id));
    }

    [Test]
    public void ExtendedEvent_ShouldPreserveCustomProperties()
    {
        // Arrange & Act
        var testValue = "TestValue";
        var @event = new ExtendedTestEvent { TestProperty = testValue };

        // Assert
        Assert.That(@event.TestProperty, Is.EqualTo(testValue));
        Assert.That(@event.Id, Is.Not.EqualTo(string.Empty));

    }

    private class TestEvent : BasePostmanEvent { }

    private class ExtendedTestEvent : BasePostmanEvent
    {
        public string TestProperty { get; set; } = string.Empty;
    }
}
