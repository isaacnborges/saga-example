using Order.Domain.Models;
using Saga.Core.Extensions;

namespace Order.Api.Dtos;

public struct OrderResponse
{
    public Guid OrderId { get; set; }
    public OrderStatus Status { get; set; }
    public string StatusDescription => Status.GetDescription();

    public static explicit operator OrderResponse(Domain.Models.Order order)
    {
        return new OrderResponse
        {
            OrderId = order.Id,
            Status = order.Status
        };
    }
}
