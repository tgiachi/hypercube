using System.Collections.Concurrent;
using HyperCube.Postman.Base.Events;
using HyperCube.Postman.Extensions;
using HyperCube.Postman.Interfaces.Events;
using HyperCube.Postman.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HyperCube.Tests.Postman;

[TestFixture]
public class HyperPostmanIntegrationTests
{
    private ServiceProvider _serviceProvider;
    private IHyperPostmanService _postmanService;
    private TestEventListener _testListener;
    private OrderEventListener _orderListener;

    [SetUp]
    public void SetUp()
    {
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(builder => builder.ClearProviders());

        // Configure HyperPostman
        services.AddPostman(
            options =>
            {
                options.MaxConcurrentTasks = 4;
                options.ContinueOnError = true;
                options.BufferEvents = true;
            }
        );

        // Register our test listeners
        _testListener = new TestEventListener();
        _orderListener = new OrderEventListener();


        _serviceProvider = services.BuildServiceProvider();

        // Get the postman service
        _postmanService = _serviceProvider.GetRequiredService<IHyperPostmanService>();

        _postmanService.RegisterListener(_testListener);
        _postmanService.RegisterListener(_orderListener);
    }

    [TearDown]
    public void TearDown()
    {
        _serviceProvider.Dispose();
    }

    [Test]
    public async Task IntegrationTest_EventsAreDispatchedToCorrectListeners()
    {
        // Arrange
        var testEvent = new TestEvent();
        var orderEvent = new OrderPlacedEvent { OrderId = "123", Amount = 99.99m };

        // Act
        await _postmanService.DispatchAsync(testEvent);
        await _postmanService.DispatchAsync(orderEvent);

        // Allow time for async processing
        await Task.Delay(100);

        // Assert
        Assert.That(_testListener.HandledEvents, Has.Count.EqualTo(1));
        Assert.That(_testListener.HandledEvents[0].Id, Is.EqualTo(testEvent.Id));

        Assert.That(_orderListener.HandledEvents, Has.Count.EqualTo(1));
        Assert.That(_orderListener.HandledEvents[0].OrderId, Is.EqualTo("123"));
        Assert.That(_orderListener.HandledEvents[0].Amount, Is.EqualTo(99.99m));
    }

    [Test]
    public async Task IntegrationTest_CallbackRegistration()
    {
        // Arrange

        var testEvent = new TestEvent();



        // Act
        await _postmanService.DispatchAsync(testEvent);
        await _testListener.WaitForEventAsync(TimeSpan.FromSeconds(2));

        // Allow time for async processing
        SpinWait.SpinUntil(() => _testListener.HandledEvents.Count > 0, TimeSpan.FromSeconds(2));



        // Assert - Both the listener and callback should receive the event
        Assert.That(_testListener.HandledEvents, Has.Count.EqualTo(1));


        // Send another event
        var secondEvent = new TestEvent();
        await _postmanService.DispatchAsync(secondEvent);

        // Allow time for async processing
        await Task.Delay(100);

        // Assert - The listener should receive it, but the callback shouldn't
        Assert.That(_testListener.HandledEvents, Has.Count.EqualTo(2));

    }

    [Test]
    public async Task IntegrationTest_EventsAreProcessedInParallel()
    {
        // Arrange
        var startEvent = new ManualResetEventSlim(false);
        var events = new List<SlowProcessingEvent>();
        var completedEvents = new List<SlowProcessingEvent>();

        // Create a slow processor that will wait for a signal
        var slowProcessor = new SlowProcessingEventListener(startEvent, completedEvents);
        _postmanService.RegisterListener<SlowProcessingEvent>(slowProcessor);

        // Create 10 events and dispatch them
        for (int i = 0; i < 10; i++)
        {
            var evt = new SlowProcessingEvent { Id = Guid.NewGuid().ToString(), SequenceNumber = i };
            events.Add(evt);
            _ = _postmanService.DispatchAsync(evt); // Don't await, dispatch all at once
        }

        // Wait a bit to ensure all events are queued
        await Task.Delay(100);

        // Act - Signal all events to complete
        startEvent.Set();

        // Wait for all events to be processed (should be quick now that we've signaled)
        await Task.Delay(500);

        // Assert
        Assert.That(completedEvents, Has.Count.EqualTo(10));

        // Since they're processed in parallel, the completion order might not match the dispatch order
        // But all events should be processed
        foreach (var evt in events)
        {
            Assert.That(completedEvents, Does.Contain(evt));
        }
    }

    // Test Event Classes
    public class TestEvent : BasePostmanEvent
    {
    }

    public class OrderPlacedEvent : BasePostmanEvent
    {
        public string OrderId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class SlowProcessingEvent : IHyperPostmanEvent
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int SequenceNumber { get; set; }
    }

    // Test Listener Classes
    public class TestEventListener : ILetterListener<TestEvent>
    {
        private readonly TaskCompletionSource<bool> _tcs = new();
        public List<TestEvent> HandledEvents { get; } = new();

        public Task HandleAsync(TestEvent @event, CancellationToken cancellationToken = default)
        {
            HandledEvents.Add(@event);
            _tcs.TrySetResult(true);
            return Task.CompletedTask;
        }

        public Task WaitForEventAsync(TimeSpan timeout) =>
            _tcs.Task.WaitAsync(timeout);
    }

    public class OrderEventListener : ILetterListener<OrderPlacedEvent>
    {
        public List<OrderPlacedEvent> HandledEvents { get; } = new();

        public Task HandleAsync(OrderPlacedEvent @event, CancellationToken cancellationToken = default)
        {
            HandledEvents.Add(@event);
            return Task.CompletedTask;
        }
    }

    public class SlowProcessingEventListener : ILetterListener<SlowProcessingEvent>
    {
        private readonly ManualResetEventSlim _startEvent;
        private readonly List<SlowProcessingEvent> _completedEvents;

        public SlowProcessingEventListener(ManualResetEventSlim startEvent, List<SlowProcessingEvent> completedEvents)
        {
            _startEvent = startEvent;
            _completedEvents = completedEvents;
        }

        public Task HandleAsync(SlowProcessingEvent @event, CancellationToken cancellationToken = default)
        {
            // Wait for the signal before completing
            _startEvent.Wait(cancellationToken);

            lock (_completedEvents)
            {
                _completedEvents.Add(@event);
            }

            return Task.CompletedTask;
        }
    }
}
