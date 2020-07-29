using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicBus.Messages;
using MagicBus.Messages.Common;
using MagicBus.Providers.Common;

namespace MagicBus.Providers.HealthCheck
{
    /// <summary>
    /// run provided tests in parallel with a specified timeout
    /// </summary>
    public class ParallelHealthCheckTester :  IHealthTester
    {
        private readonly IEnumerable<IHealthCheckTest> _tests;
        private readonly ParallelHealthCheckTesterConfig _config;
        private readonly IDateTimeProvider _dateTimeProvider;


        public ParallelHealthCheckTester(IEnumerable<IHealthCheckTest> tests, ParallelHealthCheckTesterConfig config, IDateTimeProvider dateTimeProvider)
        {
            _tests = tests;
            _config = config;
            _dateTimeProvider = dateTimeProvider;
        }

        /// <summary>
        /// make sure we catch all exceptions from tests so we can build a full result that includes all test attempts
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        static async Task<HealthCheckTestResult> RunAndCatchAllExceptions(IHealthCheckTest test, CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                return await test.RunTest(ct);
            }
            catch (Exception ex)
            {
                return new HealthCheckTestResult()
                {
                    Name = test.Name,
                    Message = ex.Message,
                    ResponseStatus = HealthCheckResponseStatus.Error
                };
            }
        }


        public async Task<HealthCheckResponse> RunHealthCheck(HealthCheckRequest request)
        {
            // if there are no tests to run, just return empty success result
            if (_tests == null || !_tests.Any())
            {
                return new HealthCheckResponse()
                {
                    AppName = _config.AppName,
                    CorrelationId = request.CorrelationId,
                    MessageDate = _dateTimeProvider.UtcNow,
                    ResponseStatus = HealthCheckResponseStatus.Success,
                    TestResults = new List<HealthCheckTestResult>()
                };
            }
            
            
            var cancellationTokenSource = new CancellationTokenSource();
            var ct = cancellationTokenSource.Token;

            // build dict of test -> runtest task
            var testTasks = new Dictionary<IHealthCheckTest, Task<HealthCheckTestResult>>();
            foreach (var test in _tests)
            {
                testTasks.Add(test, RunAndCatchAllExceptions(test, ct));
            }

            // run all tasks in parallel with a timeout.
            // https://stackoverflow.com/a/9847173
            // ReSharper disable once MethodSupportsCancellation
            await Task.WhenAny(Task.WhenAll(testTasks.Values), Task.Delay(_config.Timeout));

            // process results - which may include incomplete tasks
            var result = new HealthCheckResponse()
            {
                AppName = _config.AppName,
                CorrelationId = request.CorrelationId,
                MessageDate = _dateTimeProvider.UtcNow
            };
            foreach (var test in _tests)
            {
                var task = testTasks[test];

                if (task.IsFaulted)
                {
                    result.TestResults.Add(new HealthCheckTestResult()
                    {
                        Name = test.Name,
                        Message = task.Exception?.Message ?? "Task Faulted",
                        ResponseStatus = HealthCheckResponseStatus.Error
                    });
                }
                else if (!task.IsCompleted)
                {
                    result.TestResults.Add(new HealthCheckTestResult()
                    {
                        Name = test.Name,
                        Message = $"Timeout. No Response after {_config.Timeout.TotalSeconds} seconds",
                        ResponseStatus = HealthCheckResponseStatus.Error
                    });
                }
                else
                {
                    result.TestResults.Add(task.Result);
                }
               
            }// end foreach

            // cancel any incomplete tasks
            if (!testTasks.Values.All(t => t.IsCompleted)) cancellationTokenSource.Cancel();

            result.ResponseStatus = result.AggregateTestResults();
            result.Description = $"Ran {_tests.Count()} Tests";

            return result;
        }

    }
}
