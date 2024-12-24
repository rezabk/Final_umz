using FluentValidation;

namespace Application.ViewModels.Community.FluentValidation;

public class RequestSendCommunityMessageValidator : AbstractValidator<RequestSendCommunityMessageViewModel>
{
    public RequestSendCommunityMessageValidator()
    {
        RuleFor(x => x.CommunityId).GreaterThan(0).WithMessage("آیدی گروه باید وارد شود");

        RuleFor(x => x.Message).NotEmpty().NotNull().WithMessage("لطفا پیغام خود را وارد کنید");
    }
}