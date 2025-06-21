using AutoMapper;
using gestionale_scalzo.Model;
using gestionale_scalzo.Model.DTO;

namespace gestionale_scalzo.Utils
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            AllowNullCollections = true;
            AddGlobalIgnore("Item");
            CreateMap<ApplicationUser, UserDTO>().ReverseMap();
            //CreateMap<ApplicationUser, ApplicationUserDTO>().ReverseMap();            
            //CreateMap<ApplicationUser,ApplicationUserDTO>();
            //CreateMap<Contract, ContractDataDTO>().ReverseMap();
            //CreateMap<Contract, ContractUpdateStateDTO>().ReverseMap();      
            //CreateMap<DocInternal, DocInternalDocsDTO>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name)).ReverseMap();
            //CreateMap<LinkUtili, LinkDTO>().ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Descrizione)).ReverseMap();
            //CreateMap<Client, ClientDTO>().ForMember(dest => dest.IdLocation, opt => opt.MapFrom(src => src.IdSede)).ReverseMap();
            //CreateMap<Sede, LocationDTO>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Nome))
            //                               .ForMember(dest => dest.Enabled, opt => opt.MapFrom(src => src.Attiva))
            //                               .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Indirizzo)).ReverseMap();
            //CreateMap<Servizio, ServizioDTO>().ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Descrizione))
            //                                  .ForMember(dest => dest.Fee, opt => opt.MapFrom(src => src.Compenso))
            //                                  .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Categoria.Name))
            //                                  .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.IdCategoria)).ReverseMap();
            //CreateMap<Comunicazione, MessageDTO>().ForMember(dest => dest.State, opt => opt.MapFrom(src => src.StatoLettura))
            //                                      .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.DataInserimento))
            //                                      .ForMember(dest => dest.Object, opt => opt.MapFrom(src => src.Oggetto))
            //                                      .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Messaggio)).ReverseMap();
        }
    }
}
