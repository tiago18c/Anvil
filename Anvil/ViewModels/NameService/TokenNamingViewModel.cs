
using Anvil.Core.ViewModels;
using Anvil.Services;
using ReactiveUI;
using Solnet.Programs.Clients;
using Solnet.Programs.Models.NameService;

namespace Anvil.ViewModels.NameService
{
    public class TokenNamingViewModel : ViewModelBase
    {
        private IRpcClientProvider _rpcProvider;

        private NameServiceClient Client => new NameServiceClient(_rpcProvider.Client);

        public string Header => "Token Registry";

        public string SolDomainQuery { get; set; }

        private ReverseTokenNameRecord _reverseMintNameRecord;
        public ReverseTokenNameRecord ReverseMintNameRecord
        {
            get => _reverseMintNameRecord;
            set => this.RaiseAndSetIfChanged(ref _reverseMintNameRecord, value);
        }

        public string TokenMintQuery { get; set; }

        private TokenNameRecord _mintNameRecord;
        public TokenNameRecord MintNameRecord
        {
            get => _mintNameRecord;
            set => this.RaiseAndSetIfChanged(ref _mintNameRecord, value);
        }

        public string TickerQuery { get; set; }

        private bool _loadingTwitter;
        public bool LoadingMint
        {
            get => _loadingTwitter;
            set => this.RaiseAndSetIfChanged(ref _loadingTwitter, value);
        }

        private bool _loadingTwitterHandle;
        public bool LoadingTicker
        {
            get => _loadingTwitterHandle;
            set => this.RaiseAndSetIfChanged(ref _loadingTwitterHandle, value);
        }
        private string _data;
        public string Data
        {
            get => _data;
            set => this.RaiseAndSetIfChanged(ref _data, value);
        }

        public TokenNamingViewModel(IRpcClientProvider rpcProvider)
        {
            _rpcProvider = rpcProvider;
            _loadingTwitter = false;
        }

        public async void QueryTokenMint()
        {
            LoadingMint = true;
            var res = await Client.GetTokenInfoFromMintAsync(TokenMintQuery);

            if (res.WasSuccessful)
            {
                MintNameRecord = res.ParsedResult;
            }
            else
            {
                MintNameRecord = null;
            }


            LoadingMint = false;
        }
        public async void QueryTokenName()
        {
            LoadingTicker = true;
            var res = await Client.GetMintFromTokenTickerAsync(TickerQuery);

            if (res.WasSuccessful)
            {
                ReverseMintNameRecord = res.ParsedResult;
            }
            else
            {
                ReverseMintNameRecord = null;
            }


            LoadingTicker = false;
        }
    }
}
