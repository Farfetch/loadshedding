using Farfetch.LoadShedding.Tasks;
using Xunit;

namespace Farfetch.LoadShedding.Tests.Tasks
{
    public class TaskQueueTests
    {
        private readonly TaskQueue _target;

        public TaskQueueTests()
        {
            this._target = new TaskQueue(int.MaxValue);
        }

        [Fact]
        public void Enqueue_QueueLimitNotReached_AddToQueue()
        {
            // Arrange
            var task = new TaskItem(0);

            // Act
            this._target.Enqueue(task);

            // Assert
            Assert.Equal(1, _target.Count);
            Assert.Equal(TaskResult.Pending, task.Status);
        }

        [Fact]
        public void Enqueue_QueueLimitIsReached_RejectItem()
        {
            // Arrange
            this._target.Limit = 1;

            var firstTask = new TaskItem(0);
            var lastTask = new TaskItem(0);

            this._target.Enqueue(firstTask);

            // Act
            this._target.Enqueue(lastTask);

            // Assert
            Assert.Equal(1, _target.Count);
            Assert.Equal(TaskResult.Rejected, lastTask.Status);
        }

        [Fact]
        public void Enqueue_TaskWithHigherPriorityQueueLimitIsReached_EnqueueItemAndRejectTheLastItemWithLowerPriority()
        {
            // Arrange
            this._target.Limit = 1;

            var lowPriorityTask = new TaskItem(Priority.NonCritical);
            this._target.Enqueue(lowPriorityTask);

            var highPriorityTask = new TaskItem(0);

            // Act
            this._target.Enqueue(highPriorityTask);

            // Assert
            Assert.Equal(1, _target.Count);
            Assert.Equal(TaskResult.Pending, highPriorityTask.Status);
            Assert.Equal(TaskResult.Rejected, lowPriorityTask.Status);
        }
    }
}
