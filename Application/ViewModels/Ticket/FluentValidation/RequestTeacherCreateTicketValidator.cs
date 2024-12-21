using FluentValidation;

namespace Application.ViewModels.Ticket.FluentValidation;

public class RequestTeacherCreateTicketValidator : AbstractValidator<RequestTeacherCreateTicketViewModel>
{
    public RequestTeacherCreateTicketValidator()
    {
        RuleFor(x => x.ClassId).GreaterThan(0).WithMessage("لطفا آیدی کلاس را وارد کنید");

        RuleFor(x => x.UserId).GreaterThan(0).WithMessage("لطفا آیدی دانشجو را وارد کنید");

        RuleFor(x => x.Subject).NotEmpty().NotNull().WithMessage("لطفا عنوان تیکت را وارد کنید");
    }
}