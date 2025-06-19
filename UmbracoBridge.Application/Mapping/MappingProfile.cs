using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UmbracoBridge.Application.Features.Commands.DocumentType.Create;
using UmbracoBridge.Domain.Models;

namespace UmbracoBridge.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DocumentTypeCommand, DocumentTypeRequest>().ReverseMap();
            CreateMap<DocumentTypeResponse,DocumentType>().ReverseMap();
        }
    }
}
