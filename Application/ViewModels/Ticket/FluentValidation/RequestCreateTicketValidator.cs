using DocumentFormat.OpenXml.Office.CustomUI;
using FluentValidation;

namespace Application.ViewModels.Ticket.FluentValidation;

public class RequestCreateTicketValidator : AbstractValidator<RequestCreateTicketViewModel>
{
    public RequestCreateTicketValidator()
    {
        RuleFor(x => x.ClassId).GreaterThan(0).WithMessage("لطفا آیدی کلاس را وارد کنید");

        RuleFor(x => x.Subject).NotEmpty().NotNull().WithMessage("لطفا عنوان تیکت را وارد کنید");
    }
}