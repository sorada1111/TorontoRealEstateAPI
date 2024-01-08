using AutoMapper;
using RealEstateAPI.DTO.Addresses;
using RealEstateAPI.DTO.Agencies;
using RealEstateAPI.DTO.Features;
using RealEstateAPI.DTO.Property;
using RealEstateLibrary.Models;
using System.Runtime;

namespace RealEstateAPI.Mapping
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<Property, PropertyCreateionDto>();
            CreateMap<PropertyCreateionDto, Property>();
            CreateMap<Property, PropertyWithoutOthersAttributeDto>();
            CreateMap<Property, PropertyUpdateDto>();
            CreateMap<PropertyUpdateDto, Property>();
            CreateMap<Property,PropertyDto>();

       

            //Address
            CreateMap<Address, AddressDto>();
            CreateMap<AddressDto, Address>();

            //Agent
            CreateMap<Agent, AgentDto>();
            CreateMap<AgentDto, Agent>();
            CreateMap<Agent, AgentCreationDto>();
            CreateMap<Agent, AgentUpdateDto>();
            CreateMap<AgentUpdateDto,Agent>();

            //Feature
            CreateMap<Feature, FeatureDto>();
            CreateMap<FeatureDto, Feature>();
           


        }
    }
}
