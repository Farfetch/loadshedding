using Farfetch.LoadShedding.Events;
using Farfetch.LoadShedding.Events.Args;
using Farfetch.LoadShedding.Exceptions;
using Farfetch.LoadShedding.Tasks;
using Moq;
using Xunit;

namespace Farfetch.LoadShedding.Tests.Tasks
{
    public class TaskManagerTests
    {
        private const Priority DefaultPriority = Priority.Normal;

        [Fact]
        public void TaskManager_Initialize_TheValuesShouldBeDefined()
        {
            // Arrange
            const int ExpectedTotalCount = 10, ExpectedQueueSize = 10;

            // Act
            var target = new TaskManager(ExpectedTotalCount, ExpectedQueueSize);

            // Assert
            Assert.Equal(ExpectedTotalCount, target.ConcurrencyLimit);
            Assert.Equal(ExpectedQueueSize, target.QueueLimit);
            Assert.Equal(0, target.ConcurrencyCount);
            Assert.Equal(0, target.QueueCount);
        }

        [Fact]
        public void TaskManager_ChangeValues_TheValuesShouldBeCorrectlyDefined()
        {
            // Arrange
            const int ExpectedTotalCount = 20, ExpectedQueueSize = 20;

            var target = new TaskManager(10, 10);

            // Act
            target.ConcurrencyLimit = ExpectedTotalCount;
            target.QueueLimit = ExpectedQueueSize;

            // Assert
            Assert.Equal(ExpectedTotalCount, target.ConcurrencyLimit);
            Assert.Equal(ExpectedQueueSize, target.QueueLimit);
            Assert.Equal(0, target.ConcurrencyCount);
            Assert.Equal(0, target.QueueCount);
        }

        [Fact]
        public void TaskManager_MaxCountBelow1_ExceptionIsThrown()
        {
            // Arrange
            var target = new TaskManager(10, 10);

            const string ExpectedMessage = $"Must be greater than or equal to 1. (Parameter '{nameof(target.ConcurrencyLimit)}')";

            // Act
            var exception = Assert.Throws<ArgumentException>(() => target.ConcurrencyLimit = 0);

            // Assert
            Assert.Equal(ExpectedMessage, exception.Message);
        }

        [Fact]
        public void TaskManager_WhenMaxCountIsChanged_NotifyConcurrencyLimitChangedIsCalled()
        {
            // Arrange
            var expectedMaxCount = 20;
            var currentLimit = 0;
            var events = new LoadSheddingEvents();

            events.ConcurrencyLimitChanged.Subscribe(args => currentLimit = args.Limit);

            var target = new TaskManager(10, 10, Timeout.Infinite, events);

            // Act
            target.ConcurrencyLimit = expectedMaxCount;

            // Assert
            Assert.Equal(expectedMaxCount, currentLimit);
        }

        [Fact]
        public async Task WaitAsync_UsingAllMaxCount_TheCurrentIsZeroEqualsToMaxCount()
        {
            // Arrange
            const int MaxCount = 5;

            var target = new TaskManager(MaxCount, 5);

            // Act
            for (var i = 0; i < MaxCount; i++)
            {
                await target.AcquireAsync(DefaultPriority);
            }

            // Assert
            Assert.Equal(MaxCount, target.ConcurrencyCount);
        }

        [Fact]
        public async Task WaitAsync_WhenTheMaxCountIs0_TheQueueIsIncreased()
        {
            // Arrange
            const int MaxCount = 2, ExpectedQueueCount = 1;

            var target = new TaskManager(MaxCount, 2);

            // Act
            for (var i = 0; i < MaxCount; i++)
            {
                await target.AcquireAsync(DefaultPriority);
            }

            var waitTask = target.AcquireAsync(DefaultPriority);

            // Assert
            Assert.Equal(MaxCount, target.ConcurrencyCount);
            Assert.Equal(ExpectedQueueCount, target.QueueCount);
            Assert.False(waitTask.IsCompleted);
        }

        [Fact]
        public async Task WaitAsync_WhenMaxCountIs0AndTheSemaphoreIsNotReleased_ExceptionIsThrown()
        {
            // Arrange
            const int MaxCount = 2, ExpectedQueueCount = 0;

            var cancellationToken = new CancellationTokenSource(500);

            var target = new TaskManager(MaxCount, 2);

            for (var i = 0; i < MaxCount; i++)
            {
                await target.AcquireAsync(DefaultPriority);
            }

            // Act
            var exception = await Assert.ThrowsAsync<OperationCanceledException>(() => target.AcquireAsync(DefaultPriority, cancellationToken.Token));

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(MaxCount, target.ConcurrencyCount);
            Assert.Equal(ExpectedQueueCount, target.QueueCount);
        }

        [Fact]
        public async Task WaitAsync_WhenAvailableConcurrencyIsChanged_NotifyConcurrentItemsCountChangedIsCalled()
        {
            // Arrange
            var notifier = new Mock<ILoadSheddingEvents>();

            var itemProcessingEvent = new Mock<IEvent<ItemProcessingEventArgs>>();

            notifier.Setup(x => x.ItemProcessing).Returns(itemProcessingEvent.Object);

            var target = new TaskManager(10, 10, Timeout.Infinite, notifier.Object);

            // Act
            await target.AcquireAsync(DefaultPriority);

            // Assert
            itemProcessingEvent.Verify(x => x.Raise(It.IsAny<ItemProcessingEventArgs>()), Times.Once());
        }

        [Fact]
        public async Task WaitAsync_WhenTheQueueLimitIsReached_ExceptionIsThrowns()
        {
            // Arrange
            const string ExpectedMessage = "The maximum queue limit of 0 is reached.";

            var target = new TaskManager(0, 0);

            // Act
            var result = await Assert.ThrowsAsync<QueueLimitReachedException>(() => target.AcquireAsync(DefaultPriority));

            // Assert
            Assert.Equal(ExpectedMessage, result.Message);
        }

        [Fact]
        public async Task WaitAsync_WhenTheQueueTimeoutIsReched_ExceptionIsThrowns()
        {
            // Arrange
            const string ExpectedMessage = "The maximum queue timeout of 1 was reached.";

            var target = new TaskManager(0, 1, 1);

            // Act
            var result = await Assert.ThrowsAsync<QueueTimeoutException>(() => target.AcquireAsync(DefaultPriority));

            // Assert
            Assert.Equal(ExpectedMessage, result.Message);
        }

        [Fact]
        public async Task WaitAsync_WhenTheQueueLimitIsReached_AnEventIsRaised()
        {
            // Arrange
            var rejectedItems = new List<ItemRejectedEventArgs>();

            var events = new LoadSheddingEvents();

            events.Rejected.Subscribe(args => rejectedItems.Add(args));

            var target = new TaskManager(0, 0, Timeout.Infinite, events);

            // Act
            await Assert.ThrowsAsync<QueueLimitReachedException>(() => target.AcquireAsync(DefaultPriority));

            // Assert
            Assert.Single(rejectedItems);
        }

        [Fact]
        public async Task Release_AllTheSemaphoreConcurrentRequestsAreReleased_TheCurrentCountReturnsToTheInitialValue()
        {
            // Arrange
            const int ExpectedSemaphoreCurrentCount = 3;

            var target = new TaskManager(ExpectedSemaphoreCurrentCount, 5);

            var tasks = Enumerable.Range(0, ExpectedSemaphoreCurrentCount)
                .Select(_ => target.AcquireAsync(DefaultPriority));

            var items = await Task.WhenAll(tasks);

            // Act
            foreach (var item in items)
            {
                item.Complete();
            }

            // Assert
            Assert.Equal(0, target.ConcurrencyCount);
        }

        [Fact]
        public async Task Release_ConcurrencyItemsIsNotZero_NotifyConcurrentItemsCountChangedIsCalled()
        {
            // Arrange
            var processedItems = new List<ItemProcessedEventArgs>();
            var processingItems = new List<ItemProcessingEventArgs>();

            var events = new LoadSheddingEvents();
            events.ItemProcessed.Subscribe(args => processedItems.Add(args));
            events.ItemProcessing.Subscribe(args => processingItems.Add(args));

            var target = new TaskManager(10, 10, Timeout.Infinite, events);

            var item = await target.AcquireAsync(Priority.Critical);

            // Act
            item.Complete();

            // Assert
            Assert.Single(processingItems);
            Assert.Equal(item.Priority, processingItems.First().Priority);
            Assert.Equal(0, processingItems.First().ConcurrencyCount);
            Assert.Equal(10, processingItems.First().ConcurrencyLimit);

            Assert.Single(processedItems);
            Assert.Equal(item.Priority, processedItems.First().Priority);
            Assert.Equal(item.ProcessingTime, processedItems.First().ProcessingTime);
            Assert.Equal(0, processedItems.First().ConcurrencyCount);
            Assert.Equal(10, processedItems.First().ConcurrencyLimit);
        }

        [Fact]
        public async Task WaitAndRelease_ReleaseWhenRequestIsWaiting_TheQueueIsCleared()
        {
            // Arrange
            const int MaxCount = 2, ExpectedQueueCount1 = 1, ExpectedQueueCount2 = 0;

            var target = new TaskManager(MaxCount, 2);

            var tasks = Enumerable.Range(0, MaxCount).Select(_ => Task.Run(() => target.AcquireAsync(DefaultPriority)));

            var items = await Task.WhenAll(tasks);

            // Act
            var semaphoreWaitTask = target.AcquireAsync(DefaultPriority);

            // Assert
            Assert.Equal(MaxCount, target.ConcurrencyCount);
            Assert.Equal(ExpectedQueueCount1, target.QueueCount);
            Assert.False(semaphoreWaitTask.IsCompleted);

            // Act
            items.First().Complete();

            await Task.Delay(1);

            // Assert
            Assert.Equal(MaxCount, target.ConcurrencyCount);
            Assert.Equal(ExpectedQueueCount2, target.QueueCount);
            Assert.True(semaphoreWaitTask.IsCompleted);
        }

        [Fact]
        public async Task Acquire_WithHigherPriorityTaskAndQueueIsFull_CancelLowPriorityTask()
        {
            // Arrange
            var rejectedItems = new List<ItemRejectedEventArgs>();

            var events = new LoadSheddingEvents();

            events.Rejected.Subscribe(args => rejectedItems.Add(args));

            var target = new TaskManager(1, 1, Timeout.Infinite, events);

            // Act
            var item = await target.AcquireAsync(DefaultPriority);
            var lowPriorityTask = target.AcquireAsync(Priority.NonCritical);
            var highPriorityTask = target.AcquireAsync(Priority.Critical);

            item.Complete();

            await Assert.ThrowsAsync<QueueLimitReachedException>(async () => await lowPriorityTask);

            await highPriorityTask;

            // Assert
            Assert.True(highPriorityTask.IsCompleted);
            Assert.False(highPriorityTask.IsFaulted);
            Assert.False(highPriorityTask.IsCanceled);
            Assert.True(lowPriorityTask.IsCompleted);
            Assert.True(lowPriorityTask.IsFaulted);
            Assert.False(lowPriorityTask.IsCanceled);
            Assert.Equal(1, target.ConcurrencyCount);
            Assert.Single(rejectedItems);
            Assert.Equal(Priority.NonCritical, rejectedItems.First().Priority);
        }

        [Fact]
        public async Task Acquire_WithLowerPriorityTaskAndQueueIsFull_RejectTask()
        {
            // Arrange
            var rejectedItems = new List<ItemRejectedEventArgs>();

            var events = new LoadSheddingEvents();

            var target = new TaskManager(1, 1, Timeout.Infinite, events);

            events.Rejected.Subscribe(args => rejectedItems.Add(args));

            // Act
            var item = await target.AcquireAsync(DefaultPriority);
            var highPriorityTask = target.AcquireAsync(Priority.Critical);
            var lowPriorityTask = target.AcquireAsync(Priority.Normal);

            item.Complete();

            await Assert.ThrowsAsync<QueueLimitReachedException>(async () => await lowPriorityTask);

            await highPriorityTask;

            // Assert
            Assert.True(highPriorityTask.IsCompleted);
            Assert.False(highPriorityTask.IsFaulted);
            Assert.False(highPriorityTask.IsCanceled);
            Assert.True(lowPriorityTask.IsCompleted);
            Assert.True(lowPriorityTask.IsFaulted);
            Assert.False(lowPriorityTask.IsCanceled);
            Assert.Equal(1, target.ConcurrencyCount);
            Assert.Single(rejectedItems);
            Assert.Equal(Priority.Normal, rejectedItems.First().Priority);
        }

        [Fact]
        public async Task Acquire_MultipleTasksEnqueued_ProcessAllTasksWithSuccess()
        {
            // Arrange
            var enqueuedItemsCount = 0;
            var dequeuedItemsCount = 0;

            var numberOfTasks = 10000;

            var lockObje = new object();

            var events = new LoadSheddingEvents();

            events.ItemEnqueued.Subscribe(_ => Interlocked.Increment(ref enqueuedItemsCount));
            events.ItemDequeued.Subscribe(_ => Interlocked.Increment(ref dequeuedItemsCount));

            var target = new TaskManager(1, int.MaxValue, Timeout.Infinite, events);

            // Act
            var tasks = Enumerable.Range(0, numberOfTasks).Select(_ => Task.Run(async () =>
            {
                var item = await target.AcquireAsync((Priority)Random.Shared.Next(3));

                item.Complete();

                return true;
            }));

            var result = await Task.WhenAll(tasks);

            // Assert
            Assert.Equal(numberOfTasks, tasks.Count(t => t.Result));
            Assert.Equal(enqueuedItemsCount, dequeuedItemsCount);
        }
    }
}
