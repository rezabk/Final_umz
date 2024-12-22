using FluentValidation;

namespace Application.ViewModels.Ticket.FluentValidation;

public class RequestSendMessageValidator : AbstractValidator<RequestSendMessageViewModel>
{
    public RequestSendMessageValidator()
    {
        RuleFor(x => x.TicketId).GreaterThan(0).WithMessage("لطفا آیدی تیکت را وارد کنید");

        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Message) || x.File != null)
            .WithMessage("پیام یا فایل باید حداقل یکی پر باشد");
    }
}