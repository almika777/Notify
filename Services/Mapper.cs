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
            
            CreateMap<NotifyModel, Notify>();
            CreateMap<Notify, NotifyModel>();
        }
    }
}
