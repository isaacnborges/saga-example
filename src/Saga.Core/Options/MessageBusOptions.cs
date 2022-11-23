using System.ComponentModel.DataAnnotations;

namespace Saga.Core.Options;

public record MessageBusOptions
{
    [Required] public Uri? ConnectionString { get; init; }
    [Required, Range(1, 10)] public int RetryLimit { get; init; }
    [Required, Timestamp] public TimeSpan InitialInterval { get; init; }
    [Required, Timestamp] public TimeSpan IntervalIncrement { get; init; }
}