using Farfetch.LoadShedding.Tasks;
using Xunit;

namespace Farfetch.LoadShedding.Tests.Tasks
{
    public class TaskItemTests
    {
        [Fact]
        public async Task WaitAsync_Process_ReturnsProcessingStatus()
        {
            // Arrange
            var taskItem = new TaskItem(0);

            var waitingTask = taskItem.WaitAsync(10000, CancellationToken.None);

            // Act
            taskItem.Process();

            await waitingTask;

            // Assert
            Assert.Equal(TaskResult.Processing, taskItem.Status);
        }

        [Fact]
        public async Task WaitAsync_TimeoutReached_ReturnsCanceledStatus()
        {
            // Arrange
            var taskItem = new TaskItem(0);

            var waitingTask = taskItem.WaitAsync(1, CancellationToken.None);

            // Act
            await waitingTask;

            await Task.Delay(10);

            // Assert
            Assert.Equal(TaskResult.Timeout, taskItem.Status);
        }

        [Fact]
        public async Task WaitAsync_CancelledToken_ReturnsCanceledStatus()
        {
            // Arrange
            var taskItem = new TaskItem(0);

            using var source = new CancellationTokenSource();

            // Act
            var waitingTask = taskItem.WaitAsync(10000, source.Token);

            source.Cancel();

            await waitingTask;

            // Assert
            Assert.Equal(TaskResult.Timeout, taskItem.Status);
        }

        [Fact]
        public async Task WaitAsync_Reject_ReturnsProcessingStatus()
        {
            // Arrange
            var taskItem = new TaskItem(0);

            var waitingTask = taskItem.WaitAsync(10000, CancellationToken.None);

            // Act
            taskItem.Reject();

            await waitingTask;

            // Assert
            Assert.Equal(TaskResult.Rejected, taskItem.Status);
        }
    }
}
