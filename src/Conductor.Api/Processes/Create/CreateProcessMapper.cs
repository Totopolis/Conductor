using AutoMapper;
using Conductor.Application.Processes;
using Conductor.Domain.Processes;

namespace Conductor.Api.Processes.Create;

internal class CreateProcessMapper : Profile
{
    public CreateProcessMapper()
    {
        CreateMap<CreateProcessRequest, CreateProcessCommand>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

        CreateMap<ProcessId, Guid>()
            .ConstructUsing(dest => dest.Id);

        CreateMap<Process, CreateProcessResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number));
    }
}
