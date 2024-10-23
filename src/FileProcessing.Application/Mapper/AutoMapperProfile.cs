using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FileProcessing.Domain.DTO;
using FileProcessing.Domain.Entities;

namespace FileProcessing.Application.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<FileProcessingRecord, FileProcessingRecordDTO>()
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.StatusDetails.StatusName)); ;
        }
    }
}
