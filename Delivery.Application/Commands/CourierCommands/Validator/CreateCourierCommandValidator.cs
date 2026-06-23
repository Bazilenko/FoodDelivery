using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delivery.Application.Commands.CourierCommands.Command;
using Delivery.Application.Interfaces.Repositories;
using FluentValidation;

namespace Delivery.Application.Commands.CourierCommands.Validator
{
    public class CreateCourierCommandValidator : BaseValidator<CreateCourierCommand> 
    {
        private readonly ICourierRepository _courierRepository;

        
        public CreateCourierCommandValidator(ICourierRepository courierRepository)
        {
            _courierRepository = courierRepository;
            RuleFor(x => x.Name)
           .NotEmpty().WithMessage("Name is required")
           .MinimumLength(2).WithMessage("Name must be at least 2 characters")
           .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .Must(BeValidEmail).WithMessage("Invalid email format");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Must(BeValidPhoneNumber).WithMessage("Invalid phone number format");
        }


      
    }

    }
