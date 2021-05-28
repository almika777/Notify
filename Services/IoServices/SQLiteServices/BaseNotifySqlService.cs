using Context;
using Context.Entities;
using Microsoft.EntityFrameworkCore;

namespace Services.IoServices.SQLiteServices
{
    public class BaseNotifySqlService
    {
        protected void DetachEntity(Notify entity, NotifyDbContext context)
        {
            context.Entry(entity).State = EntityState.Detached;
            if (entity.ChatUser != null)
            {
                context.Entry(entity.ChatUser).State = EntityState.Detached;
            }
        }
    }
}
