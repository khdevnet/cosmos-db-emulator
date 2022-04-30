using System.ComponentModel.DataAnnotations;

namespace ProfileAPI.Common.Models;

public enum ErrorCode
{
    [Display(Name = "Subscription exist")]
    SubscriptionExist = 200,
}
