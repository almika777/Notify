using AutoMapper;
using Common.Models;
using Context.Entities;

namespace Services
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<ChatUserModel, ChatUser>();
            CreateMap<ChatUser, ChatUserModel>();

            CreateMap<NotifyModel, Notify>()
                .ForMember(x => x.ChatUser, x => x.MapFrom(z => z.ChatUserModel));

            CreateMap<Notify, NotifyModel>()
                .ForMember(x => x.ChatUserModel, x => x.MapFrom(z => z.ChatUser));
        }
    }
}
