
using Anvil.Services;
using Anvil.Services.Network;
using Anvil.Services.Wallets;
using System;
using ReactiveUI;
using Anvil.Services.Network.Events;
using Anvil.Core.ViewModels;
using Anvil.ViewModels.Crafter;
using Anvil.ViewModels.Wallet;
using Anvil.Models;
using Anvil.Services.Store;
using Anvil.ViewModels.MultiSignatures;
using Solnet.Programs.Clients;
using Solnet.Programs.Models.NameService;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Anvil.ViewModels.NameService
{
    public class NameServiceViewModel : ViewModelBase
    {
        private IRpcClientProvider _rpcProvider;

        public string Header => "Name Service";

        public List<ViewModelBase> Tabs { get; set; }




        public NameServiceViewModel(IRpcClientProvider rpcProvider)
        {
            _rpcProvider = rpcProvider;

            Tabs = new List<ViewModelBase>()
            {
                new SolNamingViewModel(rpcProvider),
                new TokenNamingViewModel(rpcProvider),
                new TwitterNamingViewModel(rpcProvider),
                new AllNamesViewModel(rpcProvider)

            };
        }

    }
}
