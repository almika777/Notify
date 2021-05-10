﻿using System.Threading.Tasks;
using Common.Models;

namespace Services.Services.IoServices
{
    public interface INotifyEditor
    {
        Task<bool> Edit(long chatId, Notify model);
    }
}
