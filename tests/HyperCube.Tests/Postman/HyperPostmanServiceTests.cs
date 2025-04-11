using HyperCube.Postman.Base.Events;
using HyperCube.Postman.Config;
using HyperCube.Postman.Interfaces.Services;
using HyperCube.Postman.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace HyperCube.Tests.Postman;

[TestFixture]
public class HyperPostmanServiceTests : IDisposable
{
    private Mock<ILogger<HyperPostmanService>> _loggerMock;
    private HyperPostmanConfig _config;
    private HyperPostmanService _service;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<HyperPostmanService>>();
        _config = new HyperPostmanConfig
        {
            MaxConcurrentTasks = 4,
        };


        _service = new HyperPostmanService(_loggerMock.Object, _config);
    }

    [Test]
    public void RegisterListener_ShouldAddListener()
    {
        // Arrange
        var listener = new TestEventListener();

        // Act
        _service.Subscribe<TestEvent>(listener);

        // Assert
        Assert.That(_service.GetListenerCount<TestEvent>(), Is.EqualTo(1));
    }

    [Test]
    public void RegisterMultipleListeners_ShouldAddAllListeners()
    {
        // Arrange
        var listener1 = new TestEventListener();
        var listener2 = new TestEventListener();
        var listener3 = new AnotherTestEventListener();

        // Act
        _service.Subscribe<TestEvent>(listener1);
        _service.Subscribe<TestEvent>(listener2);
        _service.Subscribe<AnotherTestEvent>(listener3);

        // Assert
        Assert.That(_service.GetListenerCount<TestEvent>(), Is.EqualTo(2));
        Assert.That(_service.GetListenerCount<AnotherTestEvent>(), Is.EqualTo(1));

    }

    [Test]
    public void UnregisterListener_ShouldRemoveListener()
    {
        // Arrange
        var listener = new TestEventListener();
        _service.Subscribe<TestEvent>(listener);

        // Act
        _service.Unsubscribe<TestEvent>(listener);

        // Assert
        Assert.That(_service.GetListenerCount<TestEvent>(), Is.EqualTo(0));
    }



    [Test]
    public async Task DispatchAsync_WithNoListeners_ShouldReturnImmediately()
    {
        // Arrange
        var testEvent = new TestEvent();

        // Act & Assert
        // This should not throw or block
        await _service.PublishAsync(testEvent);

        // We can't directly assert anything since there's no return value
        // But if we got here, it means the method returned successfully
        Assert.Pass();
    }

    [Test]
    public async Task DispatchAsync_ShouldCallAllRegisteredListeners()
    {
        // Arrange
        var testEvent = new TestEvent();
        var listener1 = new TestEventListener();
        var listener2 = new TestEventListener();

        _service.Subscribe<TestEvent>(listener1);
        _service.Subscribe<TestEvent>(listener2);

        // Act
        await _service.PublishAsync(testEvent);

        // Allow time for async processing
        await Task.Delay(100);

        // Assert
        Assert.That(listener1.HandledEvents, Does.Contain(testEvent));
        Assert.That(listener2.HandledEvents, Does.Contain(testEvent));
    }

    [Test]
    public async Task DispatchAsync_WithFailingListener_ShouldContinueWithOtherListeners()
    {
        // Arrange
        var testEvent = new TestEvent();
        var workingListener = new TestEventListener();
        var failingListener = new FailingEventListener();

        _service.Subscribe<TestEvent>(workingListener);
        _service.Subscribe<TestEvent>(failingListener);

        // Act - This should not throw since ContinueOnError is true
        await _service.PublishAsync(testEvent);

        // Allow time for async processing
        await Task.Delay(100);

        // Assert - The working listener should still have processed the event
        Assert.That(workingListener.HandledEvents, Does.Contain(testEvent));
    }


    // Test Event Classes
    private class TestEvent : BasePostmanEvent
    {
    }

    private class AnotherTestEvent : BasePostmanEvent
    {
    }

    // Test Listener Classes
    private class TestEventListener : ILetterListener<TestEvent>
    {
        public List<TestEvent> HandledEvents { get; } = new();

        public Task HandleAsync(TestEvent @event, CancellationToken cancellationToken = default)
        {
            HandledEvents.Add(@event);
            return Task.CompletedTask;
        }
    }

    private class AnotherTestEventListener : ILetterListener<AnotherTestEvent>
    {
        public List<AnotherTestEvent> HandledEvents { get; } = new();

        public Task HandleAsync(AnotherTestEvent @event, CancellationToken cancellationToken = default)
        {
            HandledEvents.Add(@event);
            return Task.CompletedTask;
        }
    }

    private class FailingEventListener : ILetterListener<TestEvent>
    {
        public Task HandleAsync(TestEvent @event, CancellationToken cancellationToken = default)
        {
            throw new Exception("Simulated failure");
        }
    }

    private class SlowEventListener : ILetterListener<TestEvent>
    {
        public bool WasCancelled { get; private set; }

        public async Task HandleAsync(TestEvent @event, CancellationToken cancellationToken = default)
        {
            try
            {
                await Task.Delay(5000, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                WasCancelled = true;
                throw;
            }
        }
    }

    public void Dispose()
    {
        _service.Dispose();
    }
}
